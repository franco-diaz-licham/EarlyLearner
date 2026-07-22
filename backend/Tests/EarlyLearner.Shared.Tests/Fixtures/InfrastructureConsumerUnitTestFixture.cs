using EarlyLearner.Application.Ports;
using EarlyLearner.Infrastructure.Messaging.Consumers;
using EarlyLearner.Shared.DocumentStoreService;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EarlyLearner.Shared.Tests.Fixtures;

public abstract class InfrastructureConsumerUnitTestFixture
{
    protected Mock<IDocumentStore> _documentStore = default!;
    protected Mock<INotificationPublisher> _notificationPublisher = default!;
    protected HouseholdInvitationEmailSentConsumer _householdInvitationEmailSentConsumer = default!;
    protected HouseholdInvitationEmailFailedConsumer _householdInvitationEmailFailedConsumer = default!;

    [SetUp]
    public void SetUp()
    {
        _documentStore = new Mock<IDocumentStore>(MockBehavior.Strict);
        _notificationPublisher = new Mock<INotificationPublisher>(MockBehavior.Strict);

        _householdInvitationEmailSentConsumer = new HouseholdInvitationEmailSentConsumer(
            _documentStore.Object,
            _notificationPublisher.Object,
            Mock.Of<ILogger<HouseholdInvitationEmailSentConsumer>>());
        _householdInvitationEmailFailedConsumer = new HouseholdInvitationEmailFailedConsumer(
            _documentStore.Object,
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