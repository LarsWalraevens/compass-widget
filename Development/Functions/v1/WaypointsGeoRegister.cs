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

public class WaypointsGeoRegister(ILogger<WaypointsGeoRegister> logger)
{
    public ILogger ILog = logger;

    public class Body
    {
        public List<Point> Points { get; set; }
        public class Point
        {
            public Guid Id { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public DateTime DateTime { get; set; }
        }
    }

    [Function(nameof(WaypointsGeoRegister))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/waypoints/geo/register")] HttpRequest request)
    {
        Properties<Body> props = new Properties<Body>(await Task.FromResult(ILog), request);

        if (props.Request.Body.Points.IsUndefined() || props.Request.Body.Points.Count > byte.MaxValue * 10 || props.Request.Body.Points.Any(x => x.Latitude.IsUndefined() || x.Longitude.IsUndefined() || x.DateTime.IsUndefined()))
        {
            return props.Services.Reply.Create("'Missing' also includes a limit of 2500 points at once", HttpsStatus.MissingParameters);
        }

        #region Pre-TVos Handling
        //Adjust timestamp minute based
        props.Request.Body.Points.ForEach(x => { x.Id = x.Id.IsUndefined() ? Guid.NewGuid() : x.Id; x.DateTime = new DateTime(x.DateTime.Year, x.DateTime.Month, x.DateTime.Day, x.DateTime.Hour, 10 * ((x.DateTime.Minute + 5) / 10) % 60, byte.MinValue, byte.MinValue, DateTimeKind.Utc); });

        Dbx.Tvos.AccessToken token = await props.Services.Specific.UseExternApi<Dbx.Tvos.AccessToken>($"https://auth.theyr.com/v1.3/License", new { key = "bogerd-martin-admin-5286-9348-5348-8047" });

        if (token.IsUndefined() || token.Token.IsUndefined())
        {
            props.Services.Log.Error("No token");
            return props.Services.Reply.Create(HttpsStatus.Error);
        }
        #endregion

        #region Retrieve TVos
        List<(Task<Dbx.Tvos.RouteData>, Body.Point)> connections = new List<(Task<Dbx.Tvos.RouteData>, Body.Point)>();

        int chunkIndex = byte.MinValue;
        foreach (Body.Point[] chunk in props.Request.Body.Points.Chunk(byte.MaxValue))
        {
            chunkIndex++;

            foreach (Body.Point point in chunk)
            {
                Task<Dbx.Tvos.RouteData> task = props.Services.Specific.UseExternApi<Dbx.Tvos.RouteData>($"https://rdas.theyr.com/v2.2/RouteData", new
                {
                    options = new
                    {
                        units = new
                        {
                            direction = "degree",
                            speed = "kn",
                            height = "meter"
                        }
                    },
                    points = new[] { new { lat = point.Latitude, lon = point.Longitude, t = point.DateTime } }
                }, $"Bearer {token.Token}");

                connections.Add((task, point));
            }

            IEnumerable<Task<Dbx.Tvos.RouteData>> tasks = connections.Select(x => x.Item1);
            while (tasks.Count(x => x.Status == TaskStatus.RanToCompletion) < tasks.Count())
            {
                await Task.Delay(333);
                _ = await Task.WhenAny(tasks);
                Console.WriteLine($"Progress Tvos Tasks: {tasks.Count(x => x.Status == TaskStatus.RanToCompletion)}/{tasks.Count()} from chunk {chunkIndex}/{props.Request.Body.Points.Chunk(byte.MaxValue).Count()}");
            }

            Dbx.Tvos.RouteData[] waypointsData = await Task.WhenAll(tasks);
        }
        #endregion

        #region Save TVos
        props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        // Create a new DataTable.
        DataTable table = new DataTable(nameof(Dbo.GeoData));
        foreach (PropertyInfo property in typeof(Dbo.GeoData).GetProperties())
        {
            table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        }

        // Create a new DataTable.
        DataTable tableLink = new DataTable(nameof(Dbo.RefinedWaypointGeoDataLink));
        foreach (PropertyInfo property in typeof(Dbo.RefinedWaypointGeoDataLink).GetProperties())
        {
            tableLink.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        }

        foreach ((Task<Dbx.Tvos.RouteData>, Body.Point) connection in connections)
        {
            Dbx.Tvos.RouteData task = connection.Item1.Result;
            Body.Point point = connection.Item2;

            if (task.IsUndefined() || task.DataPoints.IsUndefined() || task.DataPoints.Count <= byte.MinValue || task.DataPoints.IsUndefined())
            {
                continue;
            }

            Dbx.Tvos.RouteData.Data data = task.DataPoints.First().Data;

            Guid geoId = Guid.NewGuid();

            DataRow rowLink = tableLink.NewRow();
            rowLink[nameof(Dbo.RefinedWaypointGeoDataLink.RefinedWaypointId)] = point.Id;
            rowLink[nameof(Dbo.RefinedWaypointGeoDataLink.GeoDataId)] = geoId;

            if (data.IsUndefined())
            {
                continue;
            }

            DataRow row = table.NewRow();
            row[nameof(Dbo.GeoData.Id)] = geoId;
            row[nameof(Dbo.GeoData.IsPlanned)] = false;
            row[nameof(Dbo.GeoData.Timestamp)] = point.DateTime;
            row[nameof(Dbo.GeoData.Latitude)] = (decimal)point.Latitude;
            row[nameof(Dbo.GeoData.Longitude)] = (decimal)point.Longitude;
            row[nameof(Dbo.GeoData.AirTemperatureHeight)] = data.AirTemperature.Values.Average(x => x.Height).IsUndefined() ? DBNull.Value : data.AirTemperature.Values.Average(x => x.Height);
            row[nameof(Dbo.GeoData.AirTemperatureValue)] = data.AirTemperature.Values.Average(x => x.Value).IsUndefined() ? DBNull.Value : data.AirTemperature.Values.Average(x => x.Value);
            row[nameof(Dbo.GeoData.IceFractionValue)] = data.IceFraction.Value.IsUndefined() ? DBNull.Value : data.IceFraction.Value;
            row[nameof(Dbo.GeoData.OceanCurrentDepth)] = data.OceanCurrent.Values.Average(x => x.Depth).IsUndefined() ? DBNull.Value : data.OceanCurrent.Values.Average(x => x.Depth);
            row[nameof(Dbo.GeoData.OceanCurrentDirection)] = data.OceanCurrent.Values.Average(x => x.Direction).IsUndefined() ? DBNull.Value : data.OceanCurrent.Values.Average(x => x.Direction);
            row[nameof(Dbo.GeoData.OceanCurrentValue)] = data.OceanCurrent.Values.Average(x => x.Speed).IsUndefined() ? DBNull.Value : data.OceanCurrent.Values.Average(x => x.Speed);
            row[nameof(Dbo.GeoData.SeaFloorDepthValue)] = data.SeaFloorDepth.Value.IsUndefined() ? DBNull.Value : data.SeaFloorDepth.Value;
            row[nameof(Dbo.GeoData.SeaSurfaceSalinityValue)] = data.SeaSurfaceSalinity.Value.IsUndefined() ? DBNull.Value : data.SeaSurfaceSalinity.Value;
            row[nameof(Dbo.GeoData.SeaSurfaceTemperatureValue)] = data.SeaSurfaceTemperature.Value.IsUndefined() ? DBNull.Value : data.SeaSurfaceTemperature.Value;
            row[nameof(Dbo.GeoData.StandardMeteorologyMeanSeaLevelPressure)] = data.StandardMeteorology.MeanSeaLevelPressure.IsUndefined() ? DBNull.Value : data.StandardMeteorology.MeanSeaLevelPressure;
            row[nameof(Dbo.GeoData.StandardMeteorologyRelativeHumidity)] = data.StandardMeteorology.RelativeHumidity.IsUndefined() ? DBNull.Value : data.StandardMeteorology.RelativeHumidity;
            row[nameof(Dbo.GeoData.TideDirection)] = data.Tide.Direction.IsUndefined() ? DBNull.Value : data.Tide.Direction;
            row[nameof(Dbo.GeoData.TideHeight)] = data.Tide.Height.IsUndefined() ? DBNull.Value : data.Tide.Height;
            row[nameof(Dbo.GeoData.TideSpeed)] = data.Tide.Speed.IsUndefined() ? DBNull.Value : data.Tide.Speed;
            row[nameof(Dbo.GeoData.WaveSwellDirection)] = data.Wave.Swell.Direction.IsUndefined() ? DBNull.Value : data.Wave.Swell.Direction;
            row[nameof(Dbo.GeoData.WaveSwellMaximumWaveHeight)] = data.Wave.Swell.MaximumWaveHeight.IsUndefined() ? DBNull.Value : data.Wave.Swell.MaximumWaveHeight;
            row[nameof(Dbo.GeoData.WaveSwellMeanPeriod)] = data.Wave.Swell.MeanPeriod.IsUndefined() ? DBNull.Value : data.Wave.Swell.MeanPeriod;
            row[nameof(Dbo.GeoData.WaveSwellPeakDirection)] = data.Wave.Swell.PeakDirection.IsUndefined() ? DBNull.Value : data.Wave.Swell.PeakDirection;
            row[nameof(Dbo.GeoData.WaveSwellPeakPeriod)] = data.Wave.Swell.PeakPeriod.IsUndefined() ? DBNull.Value : data.Wave.Swell.PeakPeriod;
            row[nameof(Dbo.GeoData.WaveSwellSignificantWaveHeight)] = data.Wave.Swell.SignificantWaveHeight.IsUndefined() ? DBNull.Value : data.Wave.Swell.SignificantWaveHeight;
            row[nameof(Dbo.GeoData.WaveSwellZeroCrossingPeriod)] = data.Wave.Swell.ZeroCrossingPeriod.IsUndefined() ? DBNull.Value : data.Wave.Swell.ZeroCrossingPeriod;
            row[nameof(Dbo.GeoData.WaveTotalSeaDirection)] = data.Wave.TotalSea.Direction.IsUndefined() ? DBNull.Value : data.Wave.TotalSea.Direction;
            row[nameof(Dbo.GeoData.WaveTotalSeaMaximumWaveHeight)] = data.Wave.TotalSea.MaximumWaveHeight.IsUndefined() ? DBNull.Value : data.Wave.TotalSea.MaximumWaveHeight;
            row[nameof(Dbo.GeoData.WaveTotalSeaMeanPeriod)] = data.Wave.TotalSea.MeanPeriod.IsUndefined() ? DBNull.Value : data.Wave.TotalSea.MeanPeriod;
            row[nameof(Dbo.GeoData.WaveTotalSeaPeakDirection)] = data.Wave.TotalSea.PeakDirection.IsUndefined() ? DBNull.Value : data.Wave.TotalSea.PeakDirection;
            row[nameof(Dbo.GeoData.WaveTotalSeaPeakPeriod)] = data.Wave.TotalSea.PeakPeriod.IsUndefined() ? DBNull.Value : data.Wave.TotalSea.PeakPeriod;
            row[nameof(Dbo.GeoData.WaveTotalSeaSignificantWaveHeight)] = data.Wave.TotalSea.SignificantWaveHeight.IsUndefined() ? DBNull.Value : data.Wave.TotalSea.SignificantWaveHeight;
            row[nameof(Dbo.GeoData.WaveTotalSeaZeroCrossingPeriod)] = data.Wave.TotalSea.ZeroCrossingPeriod.IsUndefined() ? DBNull.Value : data.Wave.TotalSea.ZeroCrossingPeriod;
            row[nameof(Dbo.GeoData.WaveWindSeaDirection)] = data.Wave.WindSea.Direction.IsUndefined() ? DBNull.Value : data.Wave.WindSea.Direction;
            row[nameof(Dbo.GeoData.WaveWindSeaMaximumWaveHeight)] = data.Wave.WindSea.MaximumWaveHeight.IsUndefined() ? DBNull.Value : data.Wave.WindSea.MaximumWaveHeight;
            row[nameof(Dbo.GeoData.WaveWindSeaMeanPeriod)] = data.Wave.WindSea.MeanPeriod.IsUndefined() ? DBNull.Value : data.Wave.WindSea.MeanPeriod;
            row[nameof(Dbo.GeoData.WaveWindSeaPeakDirection)] = data.Wave.WindSea.PeakDirection.IsUndefined() ? DBNull.Value : data.Wave.WindSea.PeakDirection;
            row[nameof(Dbo.GeoData.WaveWindSeaPeakPeriod)] = data.Wave.WindSea.PeakPeriod.IsUndefined() ? DBNull.Value : data.Wave.WindSea.PeakPeriod;
            row[nameof(Dbo.GeoData.WaveWindSeaSignificantWaveHeight)] = data.Wave.WindSea.SignificantWaveHeight.IsUndefined() ? DBNull.Value : data.Wave.WindSea.SignificantWaveHeight;
            row[nameof(Dbo.GeoData.WaveWindSeaZeroCrossingPeriod)] = data.Wave.WindSea.ZeroCrossingPeriod.IsUndefined() ? DBNull.Value : data.Wave.WindSea.ZeroCrossingPeriod;
            table.Rows.Add(row);
        }

        Console.WriteLine($"Progress Query Tasks: Started");
        if (!await props.Services.Query.InsertElements(table))
        {
            props.Services.Log.Database(LogLevel.Error, 202407251004, nameof(WaypointsGeoRegister), $"Could not setup all refined waypoints");
        }
        if (!await props.Services.Query.InsertElements(tableLink))
        {
            props.Services.Log.Database(LogLevel.Error, 202407251005, nameof(WaypointsGeoRegister), $"Could not setup all waypoint-geo links");
        }
        Console.WriteLine($"Progress Query Tasks: Ended");
        #endregion

        if (props.Services.Query.Execute($@"WITH Duplicates AS (SELECT Id, Latitude, Longitude, Timestamp, ROW_NUMBER() OVER (PARTITION BY Latitude, Longitude, Timestamp ORDER BY Id) AS RowNumber FROM dbo.GeoData) DELETE FROM Duplicates WHERE RowNumber > 1", false).Error)
        {
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        return props.Services.Reply.Create(HttpsStatus.Success);
    }
}