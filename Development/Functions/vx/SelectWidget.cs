using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class SelectWidget(ILogger<SelectWidget> logger)
{
    public ILogger ILog = logger;

    [Function(nameof(SelectWidget))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/select/widget/{widget:regex(compass|mainengine-over-turbocharger-scatter|auxpower-over-time-by-auxengineamount)}")] HttpRequest request, string widget)
    {
        switch (widget)
        {
            case "compass":
                Properties<Methods.Methods.SelectWidgetCompassBody> selectWidgetCompassProps = new Properties<Methods.Methods.SelectWidgetCompassBody>(await Task.FromResult(ILog), request);

                Methods.Methods.SelectWidgetCompassResult selectWidgetCompassResult = await new Methods.Methods().SelectWidgetCompass(ILog, selectWidgetCompassProps.Request.Body);

                selectWidgetCompassProps.Api.AddResponseLog(selectWidgetCompassResult.Details);
                return selectWidgetCompassProps.Services.Reply.Create(new HttpsData<object>(new { Dot = selectWidgetCompassResult.Dot, Planned = selectWidgetCompassResult.Planned, Sailed = selectWidgetCompassResult.Sailed }), selectWidgetCompassResult.Condition);
            case "mainengine-over-turbocharger-scatter":
                Properties<Methods.Methods.SelectWidgetMainEngineOverTurboChargerScatterBody> selectWidgetMainEngineOverTurboChargerScatterProps = new Properties<Methods.Methods.SelectWidgetMainEngineOverTurboChargerScatterBody>(await Task.FromResult(ILog), request);

                Methods.Methods.SelectWidgetMainEngineOverTurboChargerScatterResult selectWidgetMainEngineOverTurboChargerScatterResult = await new Methods.Methods().SelectWidgetMainEngineOverTurboChargerScatter(ILog, selectWidgetMainEngineOverTurboChargerScatterProps.Request.Body);

                selectWidgetMainEngineOverTurboChargerScatterProps.Api.AddResponseLog(selectWidgetMainEngineOverTurboChargerScatterResult.Details);
                return selectWidgetMainEngineOverTurboChargerScatterProps.Services.Reply.Create(new HttpsData<object>(new { }), selectWidgetMainEngineOverTurboChargerScatterResult.Condition);
            case "auxpower-over-time-by-auxengineamount":
                Properties<Methods.Methods.SelectWidgetAuxPowerOverTimeByAuxEngineAmountBody> SelectWidgetAuxPowerOverTimeByAuxEngineAmountProps = new Properties<Methods.Methods.SelectWidgetAuxPowerOverTimeByAuxEngineAmountBody>(await Task.FromResult(ILog), request);

                Methods.Methods.SelectWidgetAuxPowerOverTimeByAuxEngineAmountResult selectWidgetAuxPowerOverTimeByAuxEngineAmountResult = await new Methods.Methods().SelectWidgetAuxPowerOverTimeByAuxEngineAmount(ILog, SelectWidgetAuxPowerOverTimeByAuxEngineAmountProps.Request.Body);

                SelectWidgetAuxPowerOverTimeByAuxEngineAmountProps.Api.AddResponseLog(selectWidgetAuxPowerOverTimeByAuxEngineAmountResult.Details);
                return SelectWidgetAuxPowerOverTimeByAuxEngineAmountProps.Services.Reply.Create(new HttpsData<object>(new { }), selectWidgetAuxPowerOverTimeByAuxEngineAmountResult.Condition);
            default:
                return default;
        }
    }
}