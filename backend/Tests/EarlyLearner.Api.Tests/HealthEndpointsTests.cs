using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EarlyLearner.Api.Tests.Fixtures;
using Shouldly;

namespace EarlyLearner.Api.Tests;

[TestFixture]
public sealed class HealthEndpointsTests : ApiEndpointTestFixture
{
    [Test]
    public async Task Health_Should_ReturnHealthy()
    {
        // Act
        var response = await Client.GetAsync("/health");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("message").GetString().ShouldBe("Healthy");
    }

    [Test]
    public async Task Readiness_Should_ReturnReady()
    {
        // Act
        var response = await Client.GetAsync("/health/ready");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("message").GetString().ShouldBe("Ready");
    }
}
