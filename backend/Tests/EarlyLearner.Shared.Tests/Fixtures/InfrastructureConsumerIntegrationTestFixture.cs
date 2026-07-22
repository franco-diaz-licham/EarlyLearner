using EarlyLearner.Application.Ports;
using EarlyLearner.Infrastructure.Messaging.Consumers;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Tests.Fakes;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace EarlyLearner.Shared.Tests.Fixtures;

public abstract class InfrastructureConsumerIntegrationTestFixture
{
    protected ITestHarness _harness = default!;
    protected InMemoryDocumentStore _documentStore = default!;
    protected InMemoryNotificationPublisher _notificationPublisher = default!;

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
        if (_harness is not null) await _harness.Stop();
        if (_serviceProvider is not null) await _serviceProvider.DisposeAsync();
    }

    protected virtual void ConfigureHostServices(IServiceCollection services)
    {
        _documentStore = new InMemoryDocumentStore();
        _notificationPublisher = new InMemoryNotificationPublisher();

        services.AddSingleton<IDocumentStore>(_documentStore);
        services.AddSingleton<INotificationPublisher>(_notificationPublisher);
        services.AddMassTransitTestHarness(configurator => {
            configurator.AddConsumer<HouseholdInvitationEmailSentConsumer>();
            configurator.AddConsumer<HouseholdInvitationEmailFailedConsumer>();
        });
    }

    protected static Mock<ConsumeContext<TMessage>> CreateContext<TMessage>(TMessage message) where TMessage : class
    {
        var context = new Mock<ConsumeContext<TMessage>>(MockBehavior.Strict);
        context
            .SetupGet(consumeContext => consumeContext.Message)
            .Returns(message);
        context
            .SetupGet(consumeContext => consumeContext.CancellationToken)
            .Returns(CancellationToken.None);

        return context;
    }
}
