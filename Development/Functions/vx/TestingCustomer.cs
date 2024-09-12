using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions.Methods;

public partial class Methods
{
    public class TestingCustomerBody
    {
        public string Username { get; set; } = "boeckmans@eye-gauge.com";
        public string Password { get; set; } = "@Eyeapi=JQW789";
    }

    public async Task<TestingCustomerResult> TestingCustomer(ILogger iLog, TestingCustomerBody body)
    {
        Properties<object> props = new Properties<object>(await Task.FromResult(iLog));

        Dbx.SeaVision.AccessToken token = await props.Services.Specific.UseExternApi<Dbx.SeaVision.AccessToken>($"https://sea.vision/api/auth/login", new
        {
            username = body.Username,
            password = body.Password
        });

        if (token.IsUndefined() || token.Token.IsUndefined())
        {
            return new TestingCustomerResult() { Condition = HttpsStatus.Error };
        }

        _ = await props.Services.Specific.UseExternApi<object>($"https://sea.vision/api/customer/{"61f393f0-4298-11ef-8ff5-9b93ff1192fa"}", default, $"Bearer {token.Token}", default, true);
        _ = await props.Services.Specific.UseExternApi<object>($"https://sea.vision/api/customer/info/{"61f393f0-4298-11ef-8ff5-9b93ff1192fa"}", default, $"Bearer {token.Token}", default, true);

        _ = await props.Services.Specific.UseExternApi<object>($"https://sea.vision/api/users/info?page=0&pageSize=16", default, $"Bearer {token.Token}", default, true);
        _ = await props.Services.Specific.UseExternApi<object>($"https://sea.vision/api/user/{"6264f360-4298-11ef-8ff5-9b93ff1192fa"}", default, $"Bearer {token.Token}", default, true);

        return new TestingCustomerResult() { Condition = HttpsStatus.Success };
    }

    public class TestingCustomerResult : Dbx.MethodResult
    {
    }
}