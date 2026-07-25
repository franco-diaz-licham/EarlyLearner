using System.Net;
using EarlyLearner.Infrastructure.Persistence;
using NUnit.Framework;
using Shouldly;

namespace EarlyLearner.Shared.Tests.Fixtures;

[NonParallelizable]
/// <summary>
/// Provides a reusable API endpoint fixture with a test database and HTTP client.
/// </summary>
public abstract class ApiEndpointTestFixture : BaseDatabaseSetup, IDisposable
{
    private ApiTestApplicationFactory? _factory;
    private HttpClient? _client;

    protected HttpClient Client => _client ?? throw new InvalidOperationException("API test client is not initialized.");

    /// <summary>
    /// Creates a test API factory and HTTP client before each endpoint test.
    /// </summary>
    [SetUp]
    public void SetUpApi()
    {
        _factory = new ApiTestApplicationFactory(ConnectionString);
        _client = _factory.CreateClient();
    }

    /// <summary>
    /// Disposes the test HTTP client and API factory after each endpoint test.
    /// </summary>
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
