using EarlyLearner.Application.Ports;
using EarlyLearner.Shared.NotificationService;
using EarlyLearner.Shared.Tests.Fixtures;
using Moq;
using Shouldly;

namespace EarlyLearner.Infrastructure.Tests.Messaging.UnitTests;

[TestFixture]
public sealed class HouseholdInvitationEmailFailedConsumerUnitTests : InfrastructureConsumerUnitTestFixture
{
    [Test]
    public async Task Consume_Should_PublishNotification_WhenNotificationDocumentExists()
    {
        // Arrange
        var message = TestData.CreateHouseholdInvitationEmailFailedEvent();
        var notification = TestData.CreateNotification(message.HouseholdId, message.InvitationId);
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
        await _householdInvitationEmailFailedConsumer.Consume(context.Object);

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
        var message = TestData.CreateHouseholdInvitationEmailFailedEvent();
        var context = CreateContext(message);

        _documentStore
            .Setup(store => store.GetAsync<NotificationDocument>(
                NotificationDocument.ContainerName,
                NotificationDocument.BuildId(message.InvitationId),
                NotificationDocument.BuildPartitionKey(message.HouseholdId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDocument?)null);

        // Act
        await _householdInvitationEmailFailedConsumer.Consume(context.Object);

        // Assert
        _documentStore.Verify(store => store.GetAsync<NotificationDocument>(NotificationDocument.ContainerName, NotificationDocument.BuildId(message.InvitationId), NotificationDocument.BuildPartitionKey(message.HouseholdId), It.IsAny<CancellationToken>()), Times.Once);
        _notificationPublisher.VerifyNoOtherCalls();
        _documentStore.VerifyNoOtherCalls();
    }
}
