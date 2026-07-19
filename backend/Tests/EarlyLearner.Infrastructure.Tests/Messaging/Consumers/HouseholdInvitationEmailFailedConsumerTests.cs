using EarlyLearner.Application.Ports;
using EarlyLearner.Infrastructure.Messaging.Consumers;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace EarlyLearner.Infrastructure.Tests.Messaging.Consumers;

[TestFixture]
public sealed class HouseholdInvitationEmailFailedConsumerTests
{
    private Mock<IDocumentStore> _documentStore = default!;
    private Mock<INotificationPublisher> _notificationPublisher = default!;
    private HouseholdInvitationEmailFailedConsumer _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _documentStore = new Mock<IDocumentStore>(MockBehavior.Strict);
        _notificationPublisher = new Mock<INotificationPublisher>(MockBehavior.Strict);

        _sut = new HouseholdInvitationEmailFailedConsumer(
            _documentStore.Object,
            _notificationPublisher.Object,
            Mock.Of<ILogger<HouseholdInvitationEmailFailedConsumer>>());
    }

    [Test]
    public async Task Consume_Should_PublishNotification_WhenNotificationDocumentExists()
    {
        // Arrange
        var message = CreateEvent();
        var notification = CreateNotification(message.HouseholdId, message.InvitationId);
        var context = CreateContext(message);
        NotificationResponse? publishedNotification = null;

        _documentStore
            .Setup(store => store.GetAsync<NotificationDocument>(
                NotificationDocument.ContainerName,
                NotificationDocument.BuildId(message.InvitationId),
                NotificationDocument.BuildPartitionKey(message.HouseholdId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);
        _notificationPublisher
            .Setup(publisher => publisher.PublishAsync(It.IsAny<NotificationResponse>(), It.IsAny<CancellationToken>()))
            .Callback<NotificationResponse, CancellationToken>((response, _) => publishedNotification = response)
            .Returns(ValueTask.CompletedTask);

        // Act
        await _sut.Consume(context.Object);

        // Assert
        publishedNotification.ShouldNotBeNull();
        publishedNotification.Id.ShouldBe(notification.InvitationId);
        publishedNotification.HouseholdId.ShouldBe(notification.HouseholdId);
        publishedNotification.Type.ShouldBe(notification.Type);
        publishedNotification.Title.ShouldBe(notification.Title);
        publishedNotification.Message.ShouldBe(notification.Message);
        publishedNotification.OccurredAt.ShouldBe(notification.OccurredAt);
        _documentStore.Verify(store => store.GetAsync<NotificationDocument>(NotificationDocument.ContainerName, NotificationDocument.BuildId(message.InvitationId), NotificationDocument.BuildPartitionKey(message.HouseholdId), It.IsAny<CancellationToken>()), Times.Once);
        _notificationPublisher.Verify(publisher => publisher.PublishAsync(It.IsAny<NotificationResponse>(), It.IsAny<CancellationToken>()), Times.Once);
        _documentStore.VerifyNoOtherCalls();
        _notificationPublisher.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Consume_Should_NotPublishNotification_WhenNotificationDocumentIsMissing()
    {
        // Arrange
        var message = CreateEvent();
        var context = CreateContext(message);

        _documentStore
            .Setup(store => store.GetAsync<NotificationDocument>(
                NotificationDocument.ContainerName,
                NotificationDocument.BuildId(message.InvitationId),
                NotificationDocument.BuildPartitionKey(message.HouseholdId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDocument?)null);

        // Act
        await _sut.Consume(context.Object);

        // Assert
        _documentStore.Verify(store => store.GetAsync<NotificationDocument>(NotificationDocument.ContainerName, NotificationDocument.BuildId(message.InvitationId), NotificationDocument.BuildPartitionKey(message.HouseholdId), It.IsAny<CancellationToken>()), Times.Once);
        _notificationPublisher.VerifyNoOtherCalls();
        _documentStore.VerifyNoOtherCalls();
    }

    private static Mock<ConsumeContext<HouseholdInvitationEmailFailedEvent>> CreateContext(HouseholdInvitationEmailFailedEvent message)
    {
        var context = new Mock<ConsumeContext<HouseholdInvitationEmailFailedEvent>>(MockBehavior.Strict);
        context
            .SetupGet(consumeContext => consumeContext.Message)
            .Returns(message);
        context
            .SetupGet(consumeContext => consumeContext.CancellationToken)
            .Returns(CancellationToken.None);

        return context;
    }

    private static HouseholdInvitationEmailFailedEvent CreateEvent()
    {
        return new HouseholdInvitationEmailFailedEvent(
            Id: Guid.NewGuid(),
            HouseholdId: Guid.NewGuid(),
            InvitationId: Guid.NewGuid(),
            Email: "carer@example.com",
            Reason: "Email service is unavailable.",
            FailedAt: DateTimeOffset.UtcNow,
            OccurredAt: DateTimeOffset.UtcNow);
    }

    private static NotificationDocument CreateNotification(Guid householdId, Guid invitationId)
    {
        return new NotificationDocument(
            Id: NotificationDocument.BuildId(invitationId),
            HouseholdId: householdId,
            InvitationId: invitationId,
            Type: "householdInvitationEmailFailed",
            Title: "Invitation email failed",
            Message: "Invitation email failed.",
            Status: NotificationDeliveryStatus.Failed,
            OccurredAt: DateTimeOffset.UtcNow);
    }
}