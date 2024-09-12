using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Data;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class WaypointsGeoRegisterManual(ILogger<WaypointsGeoRegisterManual> logger)
{
    public ILogger ILog = logger;

    public class Body
    {
        public Guid VesselId { get; set; }
    }

    [Function(nameof(WaypointsGeoRegisterManual))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/waypoints/geo/register/manual")] HttpRequest request)
    {
        Properties<Body> props = new Properties<Body>(await Task.FromResult(ILog), request);

        if (props.Request.Body.VesselId.IsUndefined())
        {
            return props.Services.Reply.Create(HttpsStatus.MissingParameters);
        }

        props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        HttpsData<List<Dbo.RefinedWaypoint>> points = props.Services.Query.GetRecords<Dbo.RefinedWaypoint>($@"SELECT TOP(2500) RefinedWaypointId, Latitude, Longitude, Timestamp FROM dbo.RefinedWaypoint WHERE VesselId={props.Request.Body.VesselId} AND PassageRouteId IS NULL ORDER BY Timestamp DESC", true);

        if (points.Error || points.HasNoData)
        {
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        _ = await props.Services.Specific.UseExternApi<object>("https://husky-absolute-naturally.ngrok-free.app/api/v1/waypoints/geo/register", new WaypointsGeoRegister.Body() { Points = points.Data.Select(x => new WaypointsGeoRegister.Body.Point() { DateTime = x.Timestamp.UtcDateTime, Id = x.RefinedWaypointId, Latitude = x.Latitude, Longitude = x.Longitude }).ToList() });

        return props.Services.Reply.Create(HttpsStatus.Success);
    }
}