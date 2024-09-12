using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Reflection;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class VesselRouteRefine(ILogger<VesselRouteRefine> logger)
{
    public ILogger ILog = logger;

    public class Body
    {
        public Guid VesselId { get; set; }
        //OR
        public Guid PassageRouteId { get; set; }
    }

    [Function(nameof(VesselRouteRefine))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/vessel/route/refine")] HttpRequest request)
    {
        Properties<Body> props = new Properties<Body>(await Task.FromResult(ILog), request);

        if ((props.Request.Body.VesselId.IsUndefined() && props.Request.Body.PassageRouteId.IsUndefined()) || (!props.Request.Body.VesselId.IsUndefined() && !props.Request.Body.PassageRouteId.IsUndefined()))
        {
            return props.Services.Reply.Create(HttpsStatus.MissingParameters);
        }

        byte maximumMinutesBetweenWaypoints = 30;
        byte minimumNauticalMilesOnSplit = 10;

        List<Dbe.RefinedWaypoint> waypoints = new List<Dbe.RefinedWaypoint>();
        int startIndex;

        if (!props.Request.Body.VesselId.IsUndefined())
        {
            props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

            HttpsData<Dbv.LastRefined> lastRefined = props.Services.Query.GetRecord<Dbv.LastRefined>($@"SELECT MAX(Timestamp) AS Timestamp, MAX(Number) AS Number FROM dbo.RefinedWaypoint WHERE PassageRouteId IS NULL AND VesselId={props.Request.Body.VesselId}", true);

            if (lastRefined.Error)
            {
                return props.Services.Reply.Create(HttpsStatus.Error);
            }
            if (lastRefined.HasNoData || lastRefined.Data.Timestamp.IsUndefined())
            {
                //Make sure to refine a large history of locations of the boat when the boat is new
                lastRefined.Data = new Dbv.LastRefined() { Timestamp = DateTimeOffset.UtcNow.AddMonths(-6), Number = byte.MinValue };
            }

            startIndex = lastRefined.Data.Number.IsUndefined() ? byte.MinValue : lastRefined.Data.Number;

            props.Database.OverwriteActiveSqlString(Settings.ChartTrackLNTestConnectionString);

            //### PROVIDER ID UNHANDLED
            HttpsData<List<Dbv.VesselWaypoint>> data = props.Services.Query.GetRecords<Dbv.VesselWaypoint>($@"SELECT dbo.VesselLocation.Speed, dbo.VesselLocation.Course AS Bearing, dbo.VesselLocation.Heading, dbo.VesselLocation.LogDate AS Timestamp, dbo.VesselLocation.Lat AS Latitude, dbo.VesselLocation.Long AS Longitude, dbo.Vessel.VesselGuid AS VesselId FROM dbo.VesselLocation INNER JOIN dbo.Vessel ON dbo.VesselLocation.VesselId = dbo.Vessel.VesselId WHERE dbo.Vessel.VesselGuid={props.Request.Body.VesselId} AND dbo.VesselLocation.ProviderId=10 AND DATEDIFF(SECOND, {lastRefined.Data.Timestamp}, dbo.VesselLocation.LogDate) > 0", true);

            if (data.Error || data.HasNoData)
            {
                return props.Services.Reply.Create(HttpsStatus.Error);
            }

            //Sort waypoints appropriately
            data.Data = data.Data.OrderBy(x => x.Timestamp).ToList();

            data.Data.ForEach(x => { waypoints.Add(new Dbe.RefinedWaypoint(x)); });
        }
        else if (!props.Request.Body.PassageRouteId.IsUndefined())
        {
            startIndex = byte.MinValue;

            props.Database.OverwriteActiveSqlString(Settings.ChartTrackCTIConnectionString);

            HttpsData<List<Dbv.PassageRouteWaypoint>> data = props.Services.Query.GetRecords<Dbv.PassageRouteWaypoint>($@"SELECT PassageRoute.PassageRouteId, RouteWaypoint.IsGreatCircle, RouteWaypoint.Latitude, RouteWaypoint.Longitude, RouteWaypoint.DistanceToNext, RouteWaypoint.DistanceToEnd, PassageRouteWaypoint.Speed, PassageRouteWaypoint.Timestamp, PassageRouteWaypoint.Bearing FROM PassageRoute INNER JOIN PassageRouteWaypoint ON PassageRouteWaypoint.PassageRouteId = PassageRoute.PassageRouteId INNER JOIN RouteWaypoint ON RouteWaypoint.RouteWaypointId = PassageRouteWaypoint.RouteWayPointId WHERE PassageRoute.PassageRouteId = {props.Request.Body.PassageRouteId} AND PassageRoute.RecordDeleted IS NULL AND PassageRouteWaypoint.RecordDeleted IS NULL AND RouteWaypoint.RecordDeleted IS NULL", true);

            if (data.Error || data.HasNoData)
            {
                return props.Services.Reply.Create(HttpsStatus.Error);
            }

            HttpsData<Dbv.Vessel> vessel = props.Services.Query.GetRecord<Dbv.Vessel>($@"SELECT VesselId FROM dbo.Voyage WHERE VoyageId=(SELECT VoyageId FROM dbo.Passage WHERE PassageId=(SELECT PassageId FROM dbo.PassageRoute WHERE PassageRouteId={props.Request.Body.PassageRouteId}))", true);

            if (vessel.Error || vessel.HasNoData)
            {
                return props.Services.Reply.Create(HttpsStatus.Error);
            }

            data.Data.ForEach(x => { x.VesselId = vessel.Data.VesselId; });

            //Sort waypoints appropiatly
            data.Data = data.Data.OrderBy(x => x.Timestamp).ToList();

            data.Data.ForEach(x => { waypoints.Add(new Dbe.RefinedWaypoint(x)); });
        }
        else
        {
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        List<Dbe.RefinedWaypoint> refinedWaypoints = new List<Dbe.RefinedWaypoint>();
        DateTimeOffset targetTime = waypoints[byte.MinValue].Timestamp.GetNextHalfHourInterval();

        for (int waypointIndex = byte.MinValue; waypointIndex < waypoints.Count; waypointIndex++)
        {
            //Get current and next waypoint with next safety
            Dbe.RefinedWaypoint currentWaypoint = waypoints[waypointIndex];
            Dbe.RefinedWaypoint nextWaypoint = waypoints[waypointIndex + (currentWaypoint == waypoints.Last() ? byte.MinValue : 1)];

            //Remove double consecutive points or add last waypoint
            bool boatIsMoving = true;
            if (currentWaypoint.Timestamp == nextWaypoint.Timestamp || (currentWaypoint.Latitude == nextWaypoint.Latitude && currentWaypoint.Longitude == nextWaypoint.Longitude) || currentWaypoint == waypoints.Last())
            {
                if (currentWaypoint == waypoints.Last())
                {
                    refinedWaypoints.Add(new Dbe.RefinedWaypoint()
                    {
                        VesselId = currentWaypoint.VesselId,
                        PassageRouteId = currentWaypoint.PassageRouteId,
                        Heading = currentWaypoint.Heading,
                        Bearing = currentWaypoint.Bearing,
                        Speed = currentWaypoint.Speed,
                        Latitude = currentWaypoint.Latitude,
                        Longitude = currentWaypoint.Longitude,
                        Timestamp = currentWaypoint.Timestamp,
                        IsAdded = false,
                        IsGreatCircle = currentWaypoint.IsGreatCircle
                    });
                    continue;
                }
                boatIsMoving = false;
            }

            //If next waypoint is to far away time based, add more points
            if (boatIsMoving)
            {
                if ((nextWaypoint.Timestamp - currentWaypoint.Timestamp).TotalMinutes > maximumMinutesBetweenWaypoints)
                {
                    //Calculate how many splits we need based on maximum time difference
                    int amountOfSplitsNeeded = (int)Math.Ceiling((nextWaypoint.Timestamp - currentWaypoint.Timestamp).TotalMinutes / maximumMinutesBetweenWaypoints);

                    float distanceToNext = (float)props.Services.Specific.HaversineDistance(currentWaypoint.Latitude, currentWaypoint.Longitude, nextWaypoint.Latitude, nextWaypoint.Longitude) / 1.82f;

                    //Adjust splits based on distance per split being to small for low speed performance
                    float distanceStep = distanceToNext / amountOfSplitsNeeded;
                    while (distanceStep <= minimumNauticalMilesOnSplit && amountOfSplitsNeeded > 2)
                    {
                        amountOfSplitsNeeded--;
                        distanceStep = distanceToNext / amountOfSplitsNeeded;
                    }

                    //Calculate all step parameters for time and distance elements
                    TimeSpan timespanStep = (nextWaypoint.Timestamp - currentWaypoint.Timestamp) / amountOfSplitsNeeded;
                    float latitudeStep = (nextWaypoint.Latitude - currentWaypoint.Latitude) / amountOfSplitsNeeded;
                    float longitudeDifference = nextWaypoint.Longitude - currentWaypoint.Longitude;
                    float longitudeStep = (Math.Abs(longitudeDifference) >= 180 ? longitudeDifference > byte.MinValue ? longitudeDifference - 360 : longitudeDifference + 360 : longitudeDifference) / amountOfSplitsNeeded;

                    //Setup each step
                    for (int stepIndex = byte.MinValue; stepIndex < amountOfSplitsNeeded; stepIndex++)
                    {
                        //Calculate actual step value to add to the original values
                        DateTimeOffset stepTimespan = currentWaypoint.Timestamp + timespanStep * stepIndex;
                        double stepLatitude = currentWaypoint.Latitude + latitudeStep * stepIndex;
                        double stepLongitude = currentWaypoint.Longitude + longitudeStep * stepIndex;

                        //Adjust actual step calculations if GreatCircle is enabled
                        if (props.Request.Body.VesselId.IsUndefined())
                        {
                            if (!currentWaypoint.IsGreatCircle.HasValue)
                            {
                                return props.Services.Reply.Create(HttpsStatus.Error);
                            }

                            if (stepIndex != byte.MinValue && currentWaypoint.IsGreatCircle.Value)
                            {
                                double fraction = (double)stepIndex / amountOfSplitsNeeded;

                                (stepLatitude, stepLongitude) = props.Services.Specific.CalculateIntermediatePoint(currentWaypoint.Latitude, currentWaypoint.Longitude, nextWaypoint.Latitude, nextWaypoint.Longitude, fraction);
                            }
                        }

                        refinedWaypoints.Add(new Dbe.RefinedWaypoint()
                        {
                            VesselId = currentWaypoint.VesselId,
                            PassageRouteId = currentWaypoint.PassageRouteId,
                            Heading = currentWaypoint.Heading,
                            Bearing = currentWaypoint.Bearing,
                            Speed = currentWaypoint.Speed,
                            Latitude = (float)stepLatitude,
                            Longitude = (float)stepLongitude,
                            Timestamp = stepTimespan,
                            IsAdded = stepIndex != byte.MinValue,
                            IsGreatCircle = currentWaypoint.IsGreatCircle,
                        });
                    }
                }
                else
                {
                    refinedWaypoints.Add(new Dbe.RefinedWaypoint()
                    {
                        VesselId = currentWaypoint.VesselId,
                        PassageRouteId = currentWaypoint.PassageRouteId,
                        Heading = currentWaypoint.Heading,
                        Bearing = currentWaypoint.Bearing,
                        Speed = currentWaypoint.Speed,
                        Latitude = currentWaypoint.Latitude,
                        Longitude = currentWaypoint.Longitude,
                        Timestamp = currentWaypoint.Timestamp,
                        IsAdded = false,
                        IsGreatCircle = currentWaypoint.IsGreatCircle
                    });
                }
            }

            if (nextWaypoint.Timestamp == targetTime)
            {
                targetTime = targetTime.GetNextHalfHourInterval();
            }

            //Add waypoints on target times
            while (nextWaypoint.Timestamp > targetTime)
            {
                (double latitude, double longitude) = props.Services.Specific.CalculateIntermediatePoint((currentWaypoint.Latitude, currentWaypoint.Longitude, currentWaypoint.Timestamp), (nextWaypoint.Latitude, nextWaypoint.Longitude, nextWaypoint.Timestamp), targetTime);

                refinedWaypoints.Add(new Dbe.RefinedWaypoint()
                {
                    VesselId = currentWaypoint.VesselId,
                    PassageRouteId = currentWaypoint.PassageRouteId,
                    Heading = currentWaypoint.Heading,
                    Bearing = currentWaypoint.Bearing,
                    Speed = currentWaypoint.Speed,
                    Latitude = (float)latitude,
                    Longitude = (float)longitude,
                    Timestamp = targetTime,
                    IsAdded = true,
                    IsGreatCircle = currentWaypoint.IsGreatCircle,
                });

                targetTime = targetTime.GetNextHalfHourInterval();
            }
        }

        refinedWaypoints = refinedWaypoints.OrderBy(x => x.Timestamp).ToList();

        //Calculate the bearing and distances for each waypoint except the last
        for (int i = byte.MinValue; i < refinedWaypoints.Count; i++)
        {
            refinedWaypoints[i].Number = startIndex + i;

            if (refinedWaypoints[i] == refinedWaypoints.Last())
            {
                continue;
            }

            Dbe.RefinedWaypoint currentWaypoint = refinedWaypoints[i];
            Dbe.RefinedWaypoint nextWaypoint = refinedWaypoints[i + 1];

            if (props.Request.Body.VesselId.IsUndefined())
            {
                refinedWaypoints[i].DistanceToNext = (float?)props.Services.Specific.HaversineDistance(currentWaypoint.Latitude, currentWaypoint.Longitude, nextWaypoint.Latitude, nextWaypoint.Longitude);
            }

            refinedWaypoints[i].Bearing = refinedWaypoints[i].Bearing.IsUndefined() ? (int)props.Services.Specific.CalculateBearing(currentWaypoint.Latitude, currentWaypoint.Longitude, nextWaypoint.Latitude, nextWaypoint.Longitude) : refinedWaypoints[i].Bearing;
        }

        if (props.Request.Body.VesselId.IsUndefined())
        {
            for (int i = byte.MinValue; i < refinedWaypoints.Count; i++)
            {
                refinedWaypoints[i].DistanceToEnd = i == byte.MinValue ? waypoints.Sum(x => x.DistanceToNext) : (float)(refinedWaypoints[i - 1].DistanceToEnd - (double)refinedWaypoints[i - 1].DistanceToNext);
                refinedWaypoints[i].DistanceToEnd = refinedWaypoints[i].DistanceToEnd <= byte.MinValue ? byte.MinValue : refinedWaypoints[i].DistanceToEnd;
            }

            props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

            //Look ahead to see if any exist
            HttpsData<object> exists = props.Services.Query.GetRecord<object>($@"SELECT TOP(1) 1 FROM dbo.RefinedWaypoint WHERE PassageRouteId = {props.Request.Body.PassageRouteId}", false);
            if (exists.Error)
            {
                return props.Services.Reply.Create(HttpsStatus.Error);
            }

            //Delete all previous connected refined waypoints for route if existing, only stop on hard failure
            if (exists.HasData)
            {
                props.Services.Query.AddSql($@"DELETE FROM dbo.RefinedWaypoint WHERE PassageRouteId = {props.Request.Body.PassageRouteId}", false);
                if (props.Services.Query.Execute().Error)
                {
                    return props.Services.Reply.Create(HttpsStatus.Error);
                }
            }
        }

        props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        // Create a new DataTable.
        DataTable table = new DataTable(nameof(Dbo.RefinedWaypoint));
        string[] ignoreProperties = new string[] { nameof(Dbo.RefinedWaypoint.RecordCreated), nameof(Dbo.RefinedWaypoint.RecordChanged), nameof(Dbo.RefinedWaypoint.RefinedWaypointId) };
        foreach (PropertyInfo property in typeof(Dbo.RefinedWaypoint).GetProperties())
        {
            if (ignoreProperties.Any(x => x == property.Name))
            {
                continue;
            }
            table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        }

        foreach (Dbe.RefinedWaypoint refinedWaypoint in refinedWaypoints)
        {
            if (refinedWaypoint.Speed.IsUndefined())
            {
                refinedWaypoint.Speed = byte.MinValue;
            }

            DataRow row = table.NewRow();
            row[nameof(Dbo.RefinedWaypoint.VesselId)] = refinedWaypoint.VesselId;
            row[nameof(Dbo.RefinedWaypoint.PassageRouteId)] = refinedWaypoint.PassageRouteId.HasValue ? refinedWaypoint.PassageRouteId.Value : DBNull.Value;
            row[nameof(Dbo.RefinedWaypoint.Number)] = refinedWaypoint.Number;
            row[nameof(Dbo.RefinedWaypoint.Timestamp)] = refinedWaypoint.Timestamp.UtcDateTime;
            row[nameof(Dbo.RefinedWaypoint.Latitude)] = refinedWaypoint.Latitude;
            row[nameof(Dbo.RefinedWaypoint.Longitude)] = refinedWaypoint.Longitude;
            row[nameof(Dbo.RefinedWaypoint.DistanceToNext)] = refinedWaypoint.DistanceToNext.HasValue && !float.IsNaN(refinedWaypoint.DistanceToNext.Value) ? refinedWaypoint.DistanceToNext.Value : DBNull.Value;
            row[nameof(Dbo.RefinedWaypoint.DistanceToEnd)] = refinedWaypoint.DistanceToEnd.HasValue && !float.IsNaN(refinedWaypoint.DistanceToEnd.Value) ? refinedWaypoint.DistanceToEnd.Value : DBNull.Value;
            row[nameof(Dbo.RefinedWaypoint.Bearing)] = refinedWaypoint.Bearing;
            row[nameof(Dbo.RefinedWaypoint.Heading)] = refinedWaypoint.Heading;
            row[nameof(Dbo.RefinedWaypoint.Speed)] = (float)refinedWaypoint.Speed;
            row[nameof(Dbo.RefinedWaypoint.IsOriginal)] = !refinedWaypoint.IsAdded;
            table.Rows.Add(row);
        }

        Console.WriteLine($"Progress Query Tasks: Started");
        if (!await props.Services.Query.InsertElements(table))
        {
            props.Services.Log.Database(LogLevel.Error, 202407251004, nameof(VesselRouteRefine), $"Could not setup all refined waypoints");
            return props.Services.Reply.Create(HttpsStatus.Error);
        }
        Console.WriteLine($"Progress Query Tasks: Ended");

        props.Api.AddResponseLog($"Created {refinedWaypoints.Count} refined waypoints based on {waypoints.Count} database points");

        foreach (Dbe.RefinedWaypoint[] waypointChunk in waypoints.Chunk(125))
        {
            props.Api.AddResponseLog($"https://www.google.com/maps/dir/{string.Join("/", waypointChunk.Select(x => $"{x.Latitude.ToString().Replace(",", ".")},{x.Longitude.ToString().Replace(",", ".")}"))}");
        }
        foreach (Dbe.RefinedWaypoint[] waypointChunk in refinedWaypoints.Chunk(125))
        {
            props.Api.AddResponseLog($"https://www.google.com/maps/dir/{string.Join("/", waypointChunk.Select(x => $"{x.Latitude.ToString().Replace(",", ".")},{x.Longitude.ToString().Replace(",", ".")}"))}");
        }

        return props.Services.Reply.Create(HttpsStatus.Success);
    }
}