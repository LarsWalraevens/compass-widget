using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions.Methods;

public partial class Methods
{
    public class SelectVesselTelemetryBody
    {
        public string VesselId { get; set; }
        public string Timestamp { get; set; }
    }

    public async Task<SelectVesselTelemetryResult> SelectVesselTelemetry(ILogger iLog, SelectVesselTelemetryBody body)
    {
        Properties<object> props = new Properties<object>(await Task.FromResult(iLog));

        if (body.Timestamp.IsUndefined() || body.VesselId.IsUndefined())
        {
            return new SelectVesselTelemetryResult() { Condition = HttpsStatus.MissingParameters };
        }

        props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        HttpsData<List<Dbo.SeaVisionEntityTelemetry>> data = props.Services.Query.GetRecords<Dbo.SeaVisionEntityTelemetry>($@"WITH RankedTelemetry AS (SELECT SeaVisionEntityTelemetryId, SeaVisionEntityId, TelemetryKey, TelemetryValue, Timestamp, ROW_NUMBER() OVER (PARTITION BY TelemetryKey ORDER BY ABS(DATEDIFF(MINUTE, Timestamp, {body.Timestamp}))) AS RowIndex FROM dbo.SeaVisionEntityTelemetry WHERE SeaVisionEntityId IN (SELECT SeaVisionEntityId FROM dbo.SeaVisionEntity WHERE CustomerId={"13814000-1DD2-11B2-8080-808080808080"})) SELECT SeaVisionEntityTelemetryId, SeaVisionEntityId, TelemetryKey, TelemetryValue, Timestamp FROM RankedTelemetry WHERE RowIndex = 1", true);

        if (data.Error)
        {
            return new SelectVesselTelemetryResult() { Condition = HttpsStatus.Error };
        }
        if (data.HasNoData)
        {
            return new SelectVesselTelemetryResult() { Condition = HttpsStatus.Success };
        }

        return new SelectVesselTelemetryResult() { Condition = HttpsStatus.Success, Data = data.Data };
    }

    public class SelectVesselTelemetryResult : Dbx.MethodResult
    {
        public List<Dbo.SeaVisionEntityTelemetry> Data { get; set; }
    }
}