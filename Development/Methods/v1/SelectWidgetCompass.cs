using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions.Methods;

public partial class Methods
{
    public class SelectWidgetCompassBody
    {
        public string VesselId { get; set; }
        public string Timestamp { get; set; }
    }

    public async Task<SelectWidgetCompassResult> SelectWidgetCompass(ILogger iLog, SelectWidgetCompassBody body)
    {
        Properties<object> props = new Properties<object>(await Task.FromResult(iLog));

        if (body.Timestamp.IsUndefined() || body.VesselId.IsUndefined())
        {
            return new SelectWidgetCompassResult() { Condition = HttpsStatus.MissingParameters };
        }

        props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        HttpsData<Dbo.RefinedWaypoint> plannedPoint = props.Services.Query.GetRecord<Dbo.RefinedWaypoint>($@"SELECT TOP(1) * FROM dbo.RefinedWaypoint WHERE VesselId={body.VesselId} AND PassageRouteId IS NULL ORDER BY ABS(DATEDIFF(SECOND, Timestamp, {body.Timestamp}))", true);

        if (plannedPoint.Error || plannedPoint.HasNoData)
        {
            return new SelectWidgetCompassResult() { Condition = HttpsStatus.Error };
        }

        HttpsData<Dbo.GeoData> plannedGeo = props.Services.Query.GetRecord<Dbo.GeoData>($@"WITH FilteredRecords AS (SELECT Id, Latitude, Longitude, Timestamp FROM dbo.GeoData WHERE ABS(DATEDIFF(MINUTE, Timestamp, {body.Timestamp})) <= 30) SELECT TOP(1) * FROM FilteredRecords ORDER BY POWER((Latitude - {plannedPoint.Data.Latitude}), 2) + POWER((Longitude - {plannedPoint.Data.Longitude}), 2)", false);

        if (plannedGeo.Error)
        {
            return new SelectWidgetCompassResult() { Condition = HttpsStatus.Error };
        }

        if (plannedGeo.HasNoData)
        {
            plannedGeo.Data = default;
        }

        HttpsData<Dbo.RefinedWaypoint> sailedPoint = props.Services.Query.GetRecord<Dbo.RefinedWaypoint>($@"SELECT TOP(1) * FROM dbo.RefinedWaypoint WHERE VesselId={body.VesselId} AND PassageRouteId IS NULL ORDER BY ABS(DATEDIFF(SECOND, Timestamp, {body.Timestamp}))", true);

        if (sailedPoint.Error || sailedPoint.HasNoData)
        {
            return new SelectWidgetCompassResult() { Condition = HttpsStatus.Error };
        }

        HttpsData<Dbo.GeoData> sailedGeo = props.Services.Query.GetRecord<Dbo.GeoData>($@"WITH FilteredRecords AS (SELECT * FROM dbo.GeoData WHERE ABS(DATEDIFF(MINUTE, Timestamp, {body.Timestamp})) <= 30) SELECT TOP(1) * FROM FilteredRecords ORDER BY POWER((Latitude - {sailedPoint.Data.Latitude}), 2) + POWER((Longitude - {sailedPoint.Data.Longitude}), 2)", false);

        if (sailedGeo.Error)
        {
            return new SelectWidgetCompassResult() { Condition = HttpsStatus.Error };
        }

        if (sailedGeo.HasNoData)
        {
            sailedGeo.Data = default;
        }

        return new SelectWidgetCompassResult() { Condition = HttpsStatus.Success, Dot = new SelectWidgetCompassResult.DotData() { Direction = props.Services.Specific.CalculateBearing(plannedPoint.Data.Latitude, plannedPoint.Data.Longitude, sailedPoint.Data.Latitude, sailedPoint.Data.Longitude), IsWarning = false }, Planned = new SelectWidgetCompassResult.CompassData() { Vessel = plannedPoint.Data, Geo = plannedGeo.Data }, Sailed = new SelectWidgetCompassResult.CompassData() { Vessel = sailedPoint.Data, Geo = sailedGeo.Data } };
    }

    public class SelectWidgetCompassResult : Dbx.MethodResult
    {
        public DotData Dot { get; set; }
        public class DotData
        {
            public double Direction { get; set; }
            public bool IsWarning { get; set; }
        }
        public CompassData Sailed { get; set; }
        public class CompassData
        {
            public Dbo.RefinedWaypoint Vessel { get; set; }
            public Dbo.GeoData Geo { get; set; }
        }
        public CompassData Planned { get; set; }
    }
}