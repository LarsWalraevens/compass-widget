using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class SelectWidgetCompass(ILogger<SelectWidgetCompass> logger)
{
    public ILogger ILog = logger;

    [Function(nameof(SelectWidgetCompass))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/select/widget/compass")] HttpRequest request)
    {
        Properties<Methods.Methods.SelectWidgetCompassBody> props = new Properties<Methods.Methods.SelectWidgetCompassBody>(await Task.FromResult(ILog), request);

        Methods.Methods.SelectWidgetCompassResult result = await new Methods.Methods().SelectWidgetCompass(ILog, props.Request.Body);

        props.Api.AddResponseLog(result.Details);
        return props.Services.Reply.Create(new HttpsData<object>(new { Dot = result.Dot, Planned = result.Planned, Sailed = result.Sailed }), result.Condition);
    }
}