using EarlyLearner.Application.Ports;
using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using EarlyLearner.Shared.Tests.Fixtures;
using Moq;
using Shouldly;

namespace EarlyLearner.Infrastructure.Tests.Messaging.Harness;

[TestFixture]
public sealed class InfrastructureConsumerTests : InfrastructureConsumerFixture
{
    [Test]
    public async Task HouseholdInvitationEmailSentEvent_Should_BeConsumedAndPublishNotification()
    {
        // Arrange
        var message = TestData.CreateHouseholdInvitationEmailSentEvent();
        var notification = TestData.CreateNotification(message.HouseholdId, message.InvitationId, NotificationDeliveryStatus.Succeeded);
        await _documentStore.UpsertAsync(NotificationDocument.ContainerName, notification, NotificationDocument.BuildPartitionKey(message.HouseholdId));
        NotificationResponse? publishedNotification = null;

        _notificationPublisher
            .Setup(publisher => publisher.PublishAsync(It.IsAny<NotificationResponse>(), It.IsAny<CancellationToken>()))
            .Callback<NotificationResponse, CancellationToken>((response, _) => publishedNotification = response)
            .Returns(ValueTask.CompletedTask);

        // Act
        await _harness.Bus.Publish(message);

        // Assert
        (await _harness.Consumed.Any<HouseholdInvitationEmailSentEvent>()).ShouldBeTrue();
        publishedNotification.ShouldNotBeNull();
        publishedNotification.Id.ShouldBe(notification.InvitationId);
        publishedNotification.HouseholdId.ShouldBe(notification.HouseholdId);
        publishedNotification.Type.ShouldBe(notification.Type);
        _notificationPublisher.Verify(publisher => publisher.PublishAsync(It.IsAny<NotificationResponse>(), It.IsAny<CancellationToken>()), Times.Once);
        _notificationPublisher.VerifyNoOtherCalls();
    }

    [Test]
    public async Task HouseholdInvitationEmailFailedEvent_Should_BeConsumedAndPublishNotification()
    {
        // Arrange
        var message = TestData.CreateHouseholdInvitationEmailFailedEvent();
        var notification = TestData.CreateNotification(message.HouseholdId, message.InvitationId, NotificationDeliveryStatus.Failed);
        await _documentStore.UpsertAsync(NotificationDocument.ContainerName, notification, NotificationDocument.BuildPartitionKey(message.HouseholdId));
        NotificationResponse? publishedNotification = null;

        _notificationPublisher
            .Setup(publisher => publisher.PublishAsync(It.IsAny<NotificationResponse>(), It.IsAny<CancellationToken>()))
            .Callback<NotificationResponse, CancellationToken>((response, _) => publishedNotification = response)
            .Returns(ValueTask.CompletedTask);

        // Act
        await _harness.Bus.Publish(message);

        // Assert
        (await _harness.Consumed.Any<HouseholdInvitationEmailFailedEvent>()).ShouldBeTrue();
        publishedNotification.ShouldNotBeNull();
        publishedNotification.Id.ShouldBe(notification.InvitationId);
        publishedNotification.HouseholdId.ShouldBe(notification.HouseholdId);
        publishedNotification.Type.ShouldBe(notification.Type);
        _notificationPublisher.Verify(publisher => publisher.PublishAsync(It.IsAny<NotificationResponse>(), It.IsAny<CancellationToken>()), Times.Once);
        _notificationPublisher.VerifyNoOtherCalls();
    }
}