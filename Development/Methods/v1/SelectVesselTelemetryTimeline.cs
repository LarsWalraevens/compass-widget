using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions.Methods;

public partial class Methods
{
    public class SelectVesselTelemetryTimelineBody
    {
        public string VesselId { get; set; }
        //MAX 5 per call
        public string[] TelemetryKeys { get; set; }
        public string TimestampStart { get; set; }
        public string TimestampEnd { get; set; }
    }

    public async Task<SelectVesselTelemetryTimelineResult> SelectVesselTelemetryTimeline(ILogger iLog, SelectVesselTelemetryTimelineBody body)
    {
        Properties<object> props = new Properties<object>(await Task.FromResult(iLog));

        if (body.TelemetryKeys.IsUndefined() || body.VesselId.IsUndefined())
        {
            return new SelectVesselTelemetryTimelineResult() { Condition = HttpsStatus.MissingParameters };
        }

        if (body.TelemetryKeys.Length > 5)
        {
            return new SelectVesselTelemetryTimelineResult() { Condition = HttpsStatus.Invalid, Details = "Only 5 keys are allowed per call." };
        }

        List<string> keys = body.TelemetryKeys.ToList();
        while (keys.Count < 5)
        {
            keys.Add(string.Empty);
        }

        props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        HttpsData<List<Dbo.SeaVisionEntityTelemetry>> data = props.Services.Query.GetRecords<Dbo.SeaVisionEntityTelemetry>($@"SELECT * FROM dbo.SeaVisionEntityTelemetry WHERE Timestamp > {body.TimestampStart.TryConvertDateTime(DateTime.UtcNow.AddYears(-1))} AND Timestamp < {body.TimestampEnd.TryConvertDateTime(DateTime.UtcNow)} AND (TelemetryKey {"="} {keys[0]} OR TelemetryKey {"="} {keys[1]} OR TelemetryKey {"="} {keys[2]} OR TelemetryKey {"="} {keys[3]} OR TelemetryKey {"="} {keys[4]}) AND SeaVisionEntityId IN (SELECT SeaVisionEntityId FROM dbo.SeaVisionEntity WHERE CustomerId={"13814000-1DD2-11B2-8080-808080808080"})", true);

        if (data.Error)
        {
            return new SelectVesselTelemetryTimelineResult() { Condition = HttpsStatus.Error };
        }
        if (data.HasNoData)
        {
            return new SelectVesselTelemetryTimelineResult() { Condition = HttpsStatus.Success };
        }

        return new SelectVesselTelemetryTimelineResult() { Condition = HttpsStatus.Success, Data = data.Data };
    }

    public class SelectVesselTelemetryTimelineResult : Dbx.MethodResult
    {
        public List<Dbo.SeaVisionEntityTelemetry> Data { get; set; }
    }
}