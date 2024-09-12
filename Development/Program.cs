using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vvoids.Api.Configuration;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

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

IHost host = new HostBuilder().ConfigureFunctionsWebApplication().ConfigureServices(static services => { _ = services.AddApplicationInsightsTelemetryWorkerService(); _ = services.ConfigureFunctionsApplicationInsights(); }).ConfigureLogging(static logging =>
{
    _ = logging.Services.Configure<LoggerFilterOptions>(static options =>
    {
        LoggerFilterRule defaultRule = options.Rules.First(static rule => rule.ProviderName
            == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
        if (defaultRule is not null)
        {
            _ = options.Rules.Remove(defaultRule);
        }
    });
}).Build();

host.Run();