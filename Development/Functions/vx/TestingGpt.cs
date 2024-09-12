using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MimeKit;
using Vvoids.Api.Base;
using Vvoids.Api.Configuration;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class TestingGpt(ILogger<TestingGpt> logger)
{
    public ILogger ILog = logger;

    [Function(nameof(TestingGpt))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.GET, Route = "v1/testing/gpt")] HttpRequest request)
    {
        Properties<object> props = new Properties<object>(await Task.FromResult(ILog), request);

        string email = await props.Services.Specific.UseExternApi<string>("https://wisepeoplestorage.blob.core.windows.net/assets/public/PositionReportTest.eml");

        using (MemoryStream memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(email)))
        {
            MimeMessage message = MimeMessage.Load(memoryStream);
            email = message.TextBody ?? message.HtmlBody;
        }

        OpenAi.GptResponse<Dbx.OpenAi.Structure.Eml<Dbx.OpenAi.Structure.MarantPositionReport>> reply = await props.Services.Specific.UseExternApi<OpenAi.GptResponse<Dbx.OpenAi.Structure.Eml<Dbx.OpenAi.Structure.MarantPositionReport>>>("https://api.openai.com/v1/chat/completions", new OpenAi.GptBody<Dbx.OpenAi.Structure.Eml<Dbx.OpenAi.Structure.MarantPositionReport>>(messages: new OpenAi.Message[] { new OpenAi.Message(OpenAi.RoleType.System, "You are a deterministic program that converts plain text emails into structured JSON objects. The plain text you process pertains to oceanic vessel reports, such as 'Bunker Reports,' 'Port Reports,' 'Position Reports,' 'Departure Reports,' and related documentation."), new OpenAi.Message(OpenAi.RoleType.User, email) }), $"Bearer {Options.Required.OpenAi.ProjectSecret}", new[] { ("OpenAI-Organization", Options.Required.OpenAi.OrganisationKey), ("OpenAI-Project", Options.Required.OpenAi.ProjectKey) }, true);

        return props.Services.Reply.Create(HttpsStatus.Success);

        //### CHANGE DBO OBJECT (MAYBE SWITCH BASED ON REPORT TYPE?)
        //#region
        //DataTable table = new DataTable(nameof(Dbo));
        //string[] ignoreProperties = new string[] { nameof(Dbo) };
        //foreach (PropertyInfo property in typeof(Dbo).GetProperties())
        //{
        //    if (ignoreProperties.Any(x => x == property.Name))
        //    {
        //        continue;
        //    }
        //    table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        //}

        //DataRow row = table.NewRow();
        //row[nameof(Dbo)] = reply.Choices.First().Message.Content.mail_body.properties.exhaust_temperatures_t8;
        //table.Rows.Add(row);
        //#endregion

        //return !await props.Services.Query.InsertElements(table, ignoreProperties)
        //    ? props.Services.Reply.Create(HttpsStatus.Error)
        //    : props.Services.Reply.Create(HttpsStatus.Success);
    }
}