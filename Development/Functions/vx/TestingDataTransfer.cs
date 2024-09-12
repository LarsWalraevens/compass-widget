using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class TestingDataTransfer(ILogger<TestingDataTransfer> logger)
{
    public ILogger ILog = logger;

    [Function(nameof(TestingDataTransfer))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.GET, Route = "v1/testing/datatransfer")] HttpRequest request)
    {
        Properties<object> props = new Properties<object>(await Task.FromResult(ILog), request);

        props.OverwriteActiveSqlString(Settings.ChartTrackLNTestConnectionString);

        HttpsData<List<Dbo.LocationSource>> selection = props.Services.Query.GetRecords<Dbo.LocationSource>($@"SELECT * FROM dbo.LocationSource", true);

        if (selection.Error || selection.HasNoData)
        {
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        props.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        //  foreach (Dbo.LocationSource item in selection.Data)
        //  {
        //      props.Services.Query.Execute($@"INSERT INTO dbo.LocationSource (
        //[Caption]
        //,[Description]
        //,[RecordCreated]
        //,[RecordChanged]
        //,[RecordDeleted]) VALUES ({item.Caption},{item.Description},{item.RecordCreated},{item.RecordChanged},{item.RecordDeleted})", true);
        //  }

        return props.Services.Reply.Create(HttpsStatus.Success);
    }

}