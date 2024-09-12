using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class VesselEntitiesRegisterByCredentials(ILogger<VesselEntitiesRegisterByCredentials> logger)
{
    public ILogger ILog = logger;

    public class Body
    {
        public string Username { get; set; } = "boeckmans@eye-gauge.com";
        public string Password { get; set; } = "@Eyeapi=JQW789";
    }

    [Function(nameof(VesselEntitiesRegisterByCredentials))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/vessel/entities/register/bycredentials")] HttpRequest request)
    {
        Properties<Body> props = new Properties<Body>(await Task.FromResult(ILog), request);
        if (!props.Request.IsAuthorized) { return props.Services.Reply.Create(HttpsStatus.Unauthorized); }

        Dbx.SeaVision.AccessToken token = await props.Services.Specific.UseExternApi<Dbx.SeaVision.AccessToken>($"https://sea.vision/api/auth/login", new
        {
            username = props.Request.Body.Username,
            password = props.Request.Body.Password
        });

        if (token.IsUndefined() || token.Token.IsUndefined())
        {
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        List<Dbx.SeaVision.EntityData.Entity> entities = new List<Dbx.SeaVision.EntityData.Entity>();
        Dbx.SeaVision.EntityData entityPage;
        do
        {
            entityPage = await props.Services.Specific.UseExternApi<Dbx.SeaVision.EntityData>($"https://sea.vision/api/user/devices?page=0&pageSize=64", default, $"Bearer {token.Token}");

            foreach (Dbx.SeaVision.EntityData.Entity entity in entityPage.Data)
            {
                if (!entity.Type.Contains("error"))
                {
                    entities.Add(entity);
                }
            }
        } while (entityPage.HasNext);

        do
        {
            entityPage = await props.Services.Specific.UseExternApi<Dbx.SeaVision.EntityData>($"https://sea.vision/api/user/assets?page=0&pageSize=64", default, $"Bearer {token.Token}", default, false, true);

            foreach (Dbx.SeaVision.EntityData.Entity entity in entityPage.Data)
            {
                if (!entity.Type.Contains("error"))
                {
                    entities.Add(entity);
                }
            }
        } while (entityPage.HasNext);

        props.Database.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        List<Dbx.SeaVision.EntityData.Entity> newEntities = new List<Dbx.SeaVision.EntityData.Entity>();
        foreach (Dbx.SeaVision.EntityData.Entity entity in entities)
        {
            HttpsData<object> entityData = props.Services.Query.GetRecord<object>($@"SELECT * FROM dbo.SeaVisionEntity WHERE EntityId={entity.Id.Id}", false);
            if (entityData.Error)
            {
                return props.Services.Reply.Create(HttpsStatus.Error);
            }
            if (entityData.HasNoData)
            {
                newEntities.Add(entity);
            }
        }

        foreach (Dbx.SeaVision.EntityData.Entity entity in newEntities)
        {
            props.Services.Query.AddSql($@"INSERT INTO dbo.SeaVisionEntity (EntityId, EntityType, EntityName, SeaVisionType, CustomerId, TenantId) VALUES ({entity.Id.Id}, {entity.Id.EntityType}, {entity.Name}, {entity.Type}, {entity.CustomerId.Id}, {entity.TenantId.Id})", true);
        }

        if (props.Services.Query.Transaction().Error)
        {
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        return props.Services.Reply.Create(HttpsStatus.Success);
    }
}