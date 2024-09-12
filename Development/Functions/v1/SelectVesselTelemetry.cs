using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class SelectVesselTelemetry(ILogger<SelectVesselTelemetry> logger)
{
    public ILogger ILog = logger;

    [Function(nameof(SelectVesselTelemetry))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/select/vessel/telemetry")] HttpRequest request)
    {
        Properties<Methods.Methods.SelectVesselTelemetryBody> props = new Properties<Methods.Methods.SelectVesselTelemetryBody>(await Task.FromResult(ILog), request);

        Methods.Methods.SelectVesselTelemetryResult result = await new Methods.Methods().SelectVesselTelemetry(ILog, props.Request.Body);

        props.Api.AddResponseLog(result.Details);
        return props.Services.Reply.Create(new HttpsData<object>(result.Data), result.Condition, true);
    }
}