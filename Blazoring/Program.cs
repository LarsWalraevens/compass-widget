using Microsoft.AspNetCore.Components;
//using ChartTrackBlazor.Components;
using Vvoids.Api.Configuration;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;
//using Widget.Forecast;
using DevExpress.Blazor;
using Blazoring.Components;
//using System.Net.Http;

//Microsoft.AspNetCore.Components.Web.EventHandlers

Settings settings = new Settings();

Options.Required.Api.Name = "BogertMartin API 8.0";
Options.Required.Api.Key = Tags.API_KEY.GetEnvironmentStringElement(string.Empty);
Options.Required.Api.AuthorizedDomains = new List<string>() { "husky-absolute-naturally.ngrok-free.app" };
Options.Required.Api.IsDevelopmentEnvironment = Tags.API_DEVELOPMENT.GetEnvironmentBooleanElement(false);
Options.Required.Api.LocalUtcAddon = Tags.API_LOCALUTCADDON.GetEnvironmentIntElement(byte.MinValue);

Options.Required.Sql.TableLog = nameof(Dbo.ErrorLog);
Options.Required.Default.LogDatabaseActionName = Settings.LogAction.LOG.ToString();
Options.Required.Method.LogDatabasePartial = settings.LogToDatabase;
Options.Required.Sql.DefaultDatabaseConnectionString = Settings.ChartTrackCTIConnectionString;

Options.Required.Sql.TableLogMail = default;
Options.Required.Default.LogMailActionName = default;
Options.Required.Method.LogMailPartial = default;
Options.Required.Azure.MailSender = default;
Options.Required.Azure.MailEndpoint = default;
Options.Required.Azure.MailKey = default;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddMvc();

// Add DevExpressBlazor services
builder.Services.AddDevExpressBlazor(static configure => configure.BootstrapVersion = BootstrapVersion.v5);

//Snippet for something extern?
//builder.Services.AddScoped<WidgetDataService>();

builder.Services.AddHttpClient();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true).UseHsts();
}

app.UseHttpsRedirection().UseStaticFiles().UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();