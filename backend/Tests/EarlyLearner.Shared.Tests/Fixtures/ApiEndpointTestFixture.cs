using System.Net;
using EarlyLearner.Infrastructure.Persistence;
using NUnit.Framework;
using Shouldly;

namespace EarlyLearner.Shared.Tests.Fixtures;

[NonParallelizable]
public abstract class ApiEndpointTestFixture : BaseDatabaseSetup, IDisposable
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

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected new DatabaseContext CreateContext() => base.CreateContext();

    protected async Task EnsureSessionAsync()
    {
        var response = await Client.PostAsync("/api/v1/identity/session", content: null);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
