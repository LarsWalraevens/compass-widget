using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class SelectVesselData(ILogger<SelectVesselData> logger)
{
    public ILogger ILog = logger;

    [Function(nameof(SelectVesselData))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/select/vessel")] HttpRequest request)
    {
        Properties<Methods.Methods.SelectVesselDataBody> props = new Properties<Methods.Methods.SelectVesselDataBody>(await Task.FromResult(ILog), request);

        Methods.Methods.SelectVesselDataResult result = await new Methods.Methods().SelectVesselData(ILog, props.Request.Body);

        props.Api.AddResponseLog(result.Details);
        return props.Services.Reply.Create(new HttpsData<object>(result.Data), result.Condition);
    }
}