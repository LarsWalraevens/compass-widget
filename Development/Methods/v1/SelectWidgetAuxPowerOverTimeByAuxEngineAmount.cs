using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions.Methods;

public partial class Methods
{
    public class SelectWidgetAuxPowerOverTimeByAuxEngineAmountBody
    {
        public string VesselId { get; set; }
    }

    public async Task<SelectWidgetAuxPowerOverTimeByAuxEngineAmountResult> SelectWidgetAuxPowerOverTimeByAuxEngineAmount(ILogger iLog, SelectWidgetAuxPowerOverTimeByAuxEngineAmountBody body)
    {
        Properties<object> props = new Properties<object>(await Task.FromResult(iLog));

        if (body.VesselId.IsUndefined())
        {
            return new SelectWidgetAuxPowerOverTimeByAuxEngineAmountResult() { Condition = HttpsStatus.MissingParameters };
        }

        props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        HttpsData<List<Dbv.AuxPowerOverTimeByAuxEngineAmountBody>> data = props.Services.Query.GetRecords<Dbv.AuxPowerOverTimeByAuxEngineAmountBody>($@"SELECT COUNT(CASE WHEN CAST(TelemetryValue AS decimal(18,3)) > 0 THEN 1 END) AS AuxEnginesAmount, SUM(CAST(TelemetryValue AS decimal(18,3))) AS TotalPower, Timestamp FROM [dbo].[SeaVisionEntityTelemetry] WHERE TelemetryKey IN ('3-kw', '2-kw', '1-kw') GROUP BY Timestamp", true);

        if (data.Error)
        {
            return new SelectWidgetAuxPowerOverTimeByAuxEngineAmountResult() { Condition = HttpsStatus.Error };
        }
        if (data.HasNoData)
        {
            return new SelectWidgetAuxPowerOverTimeByAuxEngineAmountResult() { Condition = HttpsStatus.Success };
        }

        return new SelectWidgetAuxPowerOverTimeByAuxEngineAmountResult() { Condition = HttpsStatus.Success, DataPoints = data.Data };
    }

    public class SelectWidgetAuxPowerOverTimeByAuxEngineAmountResult : Dbx.MethodResult
    {
        public List<Dbv.AuxPowerOverTimeByAuxEngineAmountBody> DataPoints { get; set; }
    }
}