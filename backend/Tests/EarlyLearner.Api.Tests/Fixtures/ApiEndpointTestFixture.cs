using System.Net;
using EarlyLearner.Shared.Tests;
using Shouldly;

namespace EarlyLearner.Api.Tests.Fixtures;

public abstract class ApiEndpointTestFixture : BaseDatabaseSetup
{
    private ApiTestApplicationFactory _factory = default!;

    protected HttpClient Client { get; private set; } = default!;

    [SetUp]
    public void SetUpApi()
    {
        _factory = new ApiTestApplicationFactory(ConnectionString);
        Client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDownApi()
    {
        Client.Dispose();
        _factory.Dispose();
    }

    protected async Task EnsureSessionAsync()
    {
        var response = await Client.PostAsync("/api/v1/identity/session", content: null);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
