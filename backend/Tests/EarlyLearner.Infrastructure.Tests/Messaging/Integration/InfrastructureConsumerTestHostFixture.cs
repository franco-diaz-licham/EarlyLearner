using EarlyLearner.Application.Ports;
using EarlyLearner.Infrastructure.Messaging.Consumers;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Tests.Fakes;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EarlyLearner.Infrastructure.Tests.Messaging.Integration;

public abstract class InfrastructureConsumerTestHostFixture
{
    protected ITestHarness _harness = default!;
    protected InMemoryDocumentStore _documentStore = default!;
    protected Mock<INotificationPublisher> _notificationPublisher = default!;

    private ServiceProvider? _serviceProvider;

    [SetUp]
    public async Task StartHarnessAsync()
    {
        var services = new ServiceCollection();
        ConfigureHostServices(services);

        _serviceProvider = services.BuildServiceProvider(true);
        _harness = _serviceProvider.GetRequiredService<ITestHarness>();
        await _harness.Start();
    }

    [TearDown]
    public async Task StopHarnessAsync()
    {
        await _harness.Stop();
        if (_serviceProvider is not null) await _serviceProvider.DisposeAsync();
    }

    protected virtual void ConfigureHostServices(IServiceCollection services)
    {
        _documentStore = new InMemoryDocumentStore();
        _notificationPublisher = new Mock<INotificationPublisher>(MockBehavior.Strict);

        services.AddSingleton<IDocumentStore>(_documentStore);
        services.AddSingleton(_notificationPublisher.Object);
        services.AddMassTransitTestHarness(configurator => {
            configurator.AddConsumer<HouseholdInvitationEmailSentConsumer>();
            configurator.AddConsumer<HouseholdInvitationEmailFailedConsumer>();
        });
    }
}

