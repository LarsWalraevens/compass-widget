using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Reflection;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class EntityTelemetryRegister(ILogger<EntityTelemetryRegister> logger)
{
    public ILogger ILog = logger;

    public class Body
    {
        public string Username { get; set; } = "boeckmans@eye-gauge.com";
        public string Password { get; set; } = "@Eyeapi=JQW789";
        public string SeaVisionEntityId { get; set; }
        public int DayShift { get; set; } = byte.MinValue;
        public bool ViewDataQuality { get; set; } = false;
        public int MinuteInterval { get; set; } = 10;
    }

    [Function(nameof(EntityTelemetryRegister))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/entity/telemetry/register")] HttpRequest request)
    {
        Properties<Body> props = new Properties<Body>(await Task.FromResult(ILog), request);
        //if (!props.Request.IsAuthorized) { return props.Services.Reply.Create(HttpsStatus.Unauthorized); }

        props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        HttpsData<Dbo.SeaVisionEntity> entityData = props.Services.Query.GetRecord<Dbo.SeaVisionEntity>($@"SELECT * FROM dbo.SeaVisionEntity WHERE SeaVisionEntityId={props.Request.Body.SeaVisionEntityId}", true);
        if (entityData.Error || entityData.HasNoData)
        {
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        DateTime retrievalDate = DateTimeOffset.UtcNow.Date.AddDays(-props.Request.Body.DayShift - 1);

        Dbx.SeaVision.AccessToken token = await props.Services.Specific.UseExternApi<Dbx.SeaVision.AccessToken>($"https://sea.vision/api/auth/login", new
        {
            username = props.Request.Body.Username,
            password = props.Request.Body.Password
        });

        if (token.IsUndefined() || token.Token.IsUndefined())
        {
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        string[] keys = await props.Services.Specific.UseExternApi<string[]>($"https://sea.vision/api/plugins/telemetry/{entityData.Data.EntityType}/{entityData.Data.EntityId}/keys/timeseries", default, $"Bearer {token.Token}");

        if (keys.IsUndefined())
        {
            return props.Services.Reply.Create(HttpsStatus.Empty);
        }

        long intervalMilliseconds = props.Request.Body.MinuteInterval * 60 * 1000;

        JObject result = await props.Services.Specific.UseExternApi<JObject>($"https://sea.vision/api/plugins/telemetry/{entityData.Data.EntityType}/{entityData.Data.EntityId}/values/timeseries?keys={string.Join(',', keys)}&startTs={((DateTimeOffset)DateTimeOffset.UtcNow.Date).AddDays(props.Request.Body.DayShift).ToUnixTimeMilliseconds()}&endTs={((DateTimeOffset)DateTimeOffset.UtcNow.Date).AddDays(props.Request.Body.DayShift + 1).ToUnixTimeMilliseconds()}&interval={intervalMilliseconds}&agg=AVG", default, $"Bearer {token.Token}", default, props.Request.Body.ViewDataQuality);

        DataTable table = new DataTable(nameof(Dbo.SeaVisionEntityTelemetry));
        string[] ignoreProperties = new string[] { nameof(Dbo.SeaVisionEntityTelemetry.SeaVisionEntityTelemetryId) };
        foreach (PropertyInfo property in typeof(Dbo.SeaVisionEntityTelemetry).GetProperties())
        {
            if (ignoreProperties.Any(x => x == property.Name))
            {
                continue;
            }
            table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        }

        List<long> telemetryTimestamps = new List<long>();

        foreach (JProperty element in result.Properties())
        {
            if (element.Value.Type != JTokenType.Array)
            {
                continue;
            }

            JArray values = (JArray)element.Value;
            if (values.IsUndefined())
            {
                continue;
            }

            foreach (JToken item in values)
            {
                if (item.Type != JTokenType.Object || item.IsUndefined())
                {
                    continue;
                }

                try
                {
                    if (props.Request.Body.ViewDataQuality)
                    {
                        telemetryTimestamps.Add((long)item["ts"]);
                        continue;
                    }

                    DataRow row = table.NewRow();
                    DateTime timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)item["ts"]).DateTime;
                    row[nameof(Dbo.SeaVisionEntityTelemetry.Timestamp)] = timestamp.AddSeconds(-timestamp.Second);
                    row[nameof(Dbo.SeaVisionEntityTelemetry.TelemetryValue)] = (string)item["value"];
                    row[nameof(Dbo.SeaVisionEntityTelemetry.SeaVisionEntityId)] = props.Request.Body.SeaVisionEntityId;
                    row[nameof(Dbo.SeaVisionEntityTelemetry.TelemetryKey)] = element.Name;
                    table.Rows.Add(row);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        if (props.Request.Body.ViewDataQuality)
        {
            telemetryTimestamps = telemetryTimestamps.OrderDescending().ToList();
            long lastTimestamp = byte.MinValue;
            List<long> totalMissed = new List<long>();
            foreach (long timestamp in telemetryTimestamps)
            {
                if (lastTimestamp == byte.MinValue)
                {
                    lastTimestamp = timestamp;
                    continue;
                }
                else
                {
                    long missed = ((lastTimestamp - timestamp) / intervalMilliseconds) - 1;
                    if (missed > byte.MinValue)
                    {
                        props.Services.Log.Debug($"MISSING: {missed} | {lastTimestamp} - {timestamp} = {(lastTimestamp - timestamp)}");
                        Console.WriteLine();
                        totalMissed.Add(missed);
                    }
                    lastTimestamp = timestamp;
                }
            }

            if (totalMissed.Count > byte.MinValue)
            {
                props.Services.Log.Debug($"---");
                props.Services.Log.Debug($"TOTAL MISSING: {totalMissed.Sum(x => x)}");
                props.Services.Log.Debug($"MAX MISSING: {totalMissed.Max(x => x)}");
            }

            return props.Services.Reply.Create(HttpsStatus.Success);
        }

        if (!await props.Services.Query.InsertElements(table))
        {
            props.Services.Log.Database(LogLevel.Error, 202408051209, nameof(EntityTelemetryRegister), $"Could not handle telemetry data");
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        if (props.Services.Query.Execute($@"WITH Duplicates AS (SELECT *, ROW_NUMBER() OVER (PARTITION BY SeaVisionEntityId, TelemetryKey, Timestamp ORDER BY SeaVisionEntityTelemetryId) AS RowNumber FROM dbo.SeaVisionEntityTelemetry WHERE SeaVisionEntityId={props.Request.Body.SeaVisionEntityId}) DELETE FROM Duplicates WHERE RowNumber > 1", false).Error)
        {
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        return props.Services.Reply.Create(HttpsStatus.Success);
    }
}