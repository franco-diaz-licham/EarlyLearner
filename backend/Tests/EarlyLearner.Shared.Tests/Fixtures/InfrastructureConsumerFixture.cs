using EarlyLearner.Application.Ports;
using EarlyLearner.Infrastructure.Messaging.Consumers;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Tests.Fakes;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EarlyLearner.Shared.Tests.Fixtures;

public abstract class InfrastructureConsumerFixture
{
    protected ITestHarness _harness = default!;
    protected InMemoryDocumentStore _documentStore = default!;
    protected Mock<IDocumentStore> _documentStoreMock = default!;
    protected Mock<INotificationPublisher> _notificationPublisher = default!;
    protected HouseholdInvitationEmailSentConsumer _householdInvitationEmailSentConsumer = default!;
    protected HouseholdInvitationEmailFailedConsumer _householdInvitationEmailFailedConsumer = default!;

    private ServiceProvider? _serviceProvider;

    [SetUp]
    public async Task StartHarnessAsync()
    {
        var services = new ServiceCollection();
        ConfigureHostServices(services);

        _serviceProvider = services.BuildServiceProvider(true);
        _harness = _serviceProvider.GetRequiredService<ITestHarness>();
        await _harness.Start();

        _householdInvitationEmailSentConsumer = CreateHouseholdInvitationEmailSentConsumer();
        _householdInvitationEmailFailedConsumer = CreateHouseholdInvitationEmailFailedConsumer();
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
        _documentStoreMock = new Mock<IDocumentStore>(MockBehavior.Strict);
        _notificationPublisher = new Mock<INotificationPublisher>(MockBehavior.Strict);

        services.AddSingleton<IDocumentStore>(_documentStore);
        services.AddSingleton(_notificationPublisher.Object);
        services.AddMassTransitTestHarness(configurator => {
            configurator.AddConsumer<HouseholdInvitationEmailSentConsumer>();
            configurator.AddConsumer<HouseholdInvitationEmailFailedConsumer>();
        });
    }

    protected HouseholdInvitationEmailSentConsumer CreateHouseholdInvitationEmailSentConsumer()
    {
        return new HouseholdInvitationEmailSentConsumer(
            _documentStoreMock.Object,
            _notificationPublisher.Object,
            Mock.Of<ILogger<HouseholdInvitationEmailSentConsumer>>());
    }

    protected HouseholdInvitationEmailFailedConsumer CreateHouseholdInvitationEmailFailedConsumer()
    {
        return new HouseholdInvitationEmailFailedConsumer(
            _documentStoreMock.Object,
            _notificationPublisher.Object,
            Mock.Of<ILogger<HouseholdInvitationEmailFailedConsumer>>());
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