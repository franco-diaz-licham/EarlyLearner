using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EarlyLearner.Api.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace EarlyLearner.Api.Tests.IdentityContext;

[TestFixture]
public sealed class IdentityEndpointsTests : ApiEndpointTestFixture
{
    [Test]
    public async Task EnsureSession_Should_CreateCurrentUserAndHousehold()
    {
        // Act
        var response = await Client.PostAsync("/api/v1/identity/session", content: null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("data").GetProperty("displayName").GetString().ShouldBe("Franco Diaz");

        await using var db = CreateContext();
        var user = await db.Users.SingleAsync();
        var household = await db.Households.SingleAsync();
        user.Email.ShouldBe("franco@example.com");
        household.Name.ShouldBe("Franco's household");
    }
}
