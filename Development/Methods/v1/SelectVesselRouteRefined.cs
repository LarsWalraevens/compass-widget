using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions.Methods;

public partial class Methods
{
    public class SelectVesselRouteRefinedBody
    {
        public string VesselId { get; set; }
        public string PassageRouteId { get; set; }
    }

    public async Task<SelectVesselRouteRefinedResult> SelectVesselRouteRefined(ILogger iLog, SelectVesselRouteRefinedBody body)
    {
        Properties<object> props = new Properties<object>(await Task.FromResult(iLog));

        if (body.VesselId.IsUndefined())
        {
            return new SelectVesselRouteRefinedResult() { Condition = HttpsStatus.MissingParameters };
        }

        props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        if (body.PassageRouteId.IsUndefined())
        {
            HttpsData<List<Dbo.RefinedWaypoint>> sailed = props.Services.Query.GetRecords<Dbo.RefinedWaypoint>($@"SELECT * FROM dbo.RefinedWaypoint WHERE VesselId={body.VesselId} AND PassageRouteId IS NULL", true);

            if (sailed.Error || sailed.HasNoData)
            {
                return new SelectVesselRouteRefinedResult() { Condition = HttpsStatus.Error };
            }

            return new SelectVesselRouteRefinedResult() { Condition = HttpsStatus.Success, Sailed = sailed.Data };
        }

        HttpsData<Dbv.Timerange> timerange = props.Services.Query.GetRecord<Dbv.Timerange>($@"SELECT MIN(Timestamp) AS Minimum, MAX(Timestamp) AS Maximum FROM dbo.RefinedWaypoint WHERE VesselId={body.VesselId} AND PassageRouteId = {body.PassageRouteId}", true);

        if (timerange.Error || timerange.HasNoData)
        {
            return new SelectVesselRouteRefinedResult() { Condition = HttpsStatus.Error };
        }

        HttpsData<List<Dbo.RefinedWaypoint>> refined = props.Services.Query.GetRecords<Dbo.RefinedWaypoint>($@"SELECT * FROM dbo.RefinedWaypoint WHERE VesselId={body.VesselId} AND (PassageRouteId IS NULL OR PassageRouteId = {body.PassageRouteId}) AND Timestamp > {timerange.Data.Minimum} AND Timestamp < {timerange.Data.Maximum}", true);

        if (refined.Error || refined.HasNoData)
        {
            return new SelectVesselRouteRefinedResult() { Condition = HttpsStatus.Error };
        }

        return new SelectVesselRouteRefinedResult() { Condition = HttpsStatus.Success, Sailed = refined.Data.Where(x => x.PassageRouteId.IsUndefined()), Planned = refined.Data.Where(x => !x.PassageRouteId.IsUndefined()) };
    }

    public class SelectVesselRouteRefinedResult : Dbx.MethodResult
    {
        public IEnumerable<Dbo.RefinedWaypoint> Sailed { get; set; }
        public IEnumerable<Dbo.RefinedWaypoint> Planned { get; set; }
    }
}