using System.Net;
using EarlyLearner.Infrastructure.Persistence;
using EarlyLearner.Shared.Tests;
using Shouldly;

namespace EarlyLearner.Api.Tests.Fixtures;

[NonParallelizable]
public abstract class ApiEndpointTestFixture : BaseDatabaseSetup
{
    private ApiTestApplicationFactory? _factory;
    private HttpClient? _client;

    protected HttpClient Client => _client ?? throw new InvalidOperationException("API test client is not initialized.");

    [SetUp]
    public void SetUpApi()
    {
        _factory = new ApiTestApplicationFactory(ConnectionString);
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDownApi()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    protected new DatabaseContext CreateContext() => base.CreateContext();

    protected async Task EnsureSessionAsync()
    {
        var response = await Client.PostAsync("/api/v1/identity/session", content: null);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
