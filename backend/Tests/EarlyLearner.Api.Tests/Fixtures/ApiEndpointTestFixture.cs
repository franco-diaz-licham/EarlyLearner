using EarlyLearner.Shared.Tests;

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
}
