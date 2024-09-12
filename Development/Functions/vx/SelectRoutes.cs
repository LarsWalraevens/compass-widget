//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Metadata;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.Logging;
//using System.Data;
//using Vvoids.Api.Base;
//using Vvoids.Api.Services;

//namespace Vvoids.Api.Functions;

//public class SelectRoutes(ILogger<SelectRoutes> logger, IEnumerable<EndpointDataSource> endpointsDataSources)
//{
//    public ILogger ILog = logger;
//    public IEnumerable<EndpointDataSource> IEndpoints = endpointsDataSources;

//    [Function(nameof(SelectRoutes))]
//    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.GET, Route = "v1/select/routes")] HttpRequest request)
//    {
//        Properties<object> props = new Properties<object>(await Task.FromResult(ILog), request);

//        return props.Services.Reply.Create(new HttpsData<IEnumerable<string>>(IEndpoints.SelectMany(x => x.Endpoints).OfType<RouteEndpoint>().Select(x => $"[{string.Join(',', x.Metadata?.OfType<HttpMethodMetadata>()?.FirstOrDefault()?.HttpMethods)}] {x.Metadata?.OfType<IRouteDiagnosticsMetadata>()?.FirstOrDefault().Route}")), HttpsStatus.Success);
//    }
//}