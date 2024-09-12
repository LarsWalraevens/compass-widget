using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class SelectVesselTelemetryTimeline(ILogger<SelectVesselTelemetryTimeline> logger)
{
    public ILogger ILog = logger;

    [Function(nameof(SelectVesselTelemetryTimeline))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/select/vessel/telemetry/timeline")] HttpRequest request)
    {
        Properties<Methods.Methods.SelectVesselTelemetryTimelineBody> props = new Properties<Methods.Methods.SelectVesselTelemetryTimelineBody>(await Task.FromResult(ILog), request);

        Methods.Methods.SelectVesselTelemetryTimelineResult result = await new Methods.Methods().SelectVesselTelemetryTimeline(ILog, props.Request.Body);

        props.Api.AddResponseLog(result.Details);
        return props.Services.Reply.Create(new HttpsData<object>(result.Data), result.Condition, true);
    }
}