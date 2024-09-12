using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions.Methods;

public partial class Methods
{
    public class SelectWidgetMainEngineOverTurboChargerScatterBody
    {
        public string VesselId { get; set; }
    }

    public async Task<SelectWidgetMainEngineOverTurboChargerScatterResult> SelectWidgetMainEngineOverTurboChargerScatter(ILogger iLog, SelectWidgetMainEngineOverTurboChargerScatterBody body)
    {
        Properties<object> props = new Properties<object>(await Task.FromResult(iLog));

        if (body.VesselId.IsUndefined())
        {
            return new SelectWidgetMainEngineOverTurboChargerScatterResult() { Condition = HttpsStatus.MissingParameters };
        }

        props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        HttpsData<List<Dbo.SeaVisionEntityTelemetry>> data = props.Services.Query.GetRecords<Dbo.SeaVisionEntityTelemetry>($@"SELECT * FROM dbo.SeaVisionEntityTelemetry WHERE TelemetryKey in ('turbo-rpm','me-rpm')", true);

        if (data.Error)
        {
            return new SelectWidgetMainEngineOverTurboChargerScatterResult() { Condition = HttpsStatus.Error };
        }
        if (data.HasNoData)
        {
            return new SelectWidgetMainEngineOverTurboChargerScatterResult() { Condition = HttpsStatus.Success };
        }

        List<(string, DateTime)> turbo = data.Data.Where(x => x.TelemetryKey == "turbo-rpm").Select(x => (x.TelemetryKey, x.Timestamp)).ToList();
        List<(string, DateTime)> main = data.Data.Where(x => x.TelemetryKey == "me-rpm").Select(x => (x.TelemetryKey, x.Timestamp)).ToList();

        List<(string, string)> points = new List<(string, string)>();
        foreach ((string, DateTime) item in main)
        {
            (string, DateTime) turboItem = turbo.FirstOrDefault(x => x.Item2 == item.Item2);

            if (turboItem.IsUndefined())
            {
                points.Add((item.Item1, turboItem.Item1));
            }
        }

        return new SelectWidgetMainEngineOverTurboChargerScatterResult() { Condition = HttpsStatus.Success, Points = points };
    }

    public class SelectWidgetMainEngineOverTurboChargerScatterResult : Dbx.MethodResult
    {
        public List<(string, string)> Points { get; set; }
    }
}