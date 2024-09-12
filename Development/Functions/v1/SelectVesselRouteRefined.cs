using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class SelectVesselRouteRefined(ILogger<SelectVesselRouteRefined> logger)
{
    public ILogger ILog = logger;

    [Function(nameof(SelectVesselRouteRefined))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/select/vessel/route/refined")] HttpRequest request)
    {
        Properties<Methods.Methods.SelectVesselRouteRefinedBody> props = new Properties<Methods.Methods.SelectVesselRouteRefinedBody>(await Task.FromResult(ILog), request);

        Methods.Methods.SelectVesselRouteRefinedResult result = await new Methods.Methods().SelectVesselRouteRefined(ILog, props.Request.Body);

        props.Api.AddResponseLog(result.Details);
        return props.Services.Reply.Create(new HttpsData<object>(new { Sailed = result.Sailed, Planned = result.Planned }), result.Condition, true);
    }
}