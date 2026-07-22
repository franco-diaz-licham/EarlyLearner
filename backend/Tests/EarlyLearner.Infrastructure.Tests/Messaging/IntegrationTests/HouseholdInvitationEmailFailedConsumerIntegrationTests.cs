using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using EarlyLearner.Shared.Tests.Fixtures;
using Shouldly;

namespace EarlyLearner.Infrastructure.Tests.Messaging.Consumers;

[TestFixture]
public sealed class HouseholdInvitationEmailFailedConsumerIntegrationTests : InfrastructureConsumerIntegrationTestFixture
{
    [Test]
    public async Task Consume_Should_PublishNotification_WhenNotificationDocumentExists()
    {
        // Arrange
        var message = TestData.CreateHouseholdInvitationEmailFailedEvent();
        var notification = TestData.CreateNotification(message.HouseholdId, message.InvitationId, NotificationDeliveryStatus.Failed);
        await _documentStore.UpsertAsync(NotificationDocument.ContainerName, notification, NotificationDocument.BuildPartitionKey(message.HouseholdId));

        // Act
        await _harness.Bus.Publish(message);

        // Assert
        (await _harness.Consumed.Any<HouseholdInvitationEmailFailedEvent>()).ShouldBeTrue();

        var publishedNotification = _notificationPublisher.Notifications.SingleOrDefault();
        publishedNotification.ShouldNotBeNull();
        publishedNotification.Id.ShouldBe(notification.InvitationId);
        publishedNotification.HouseholdId.ShouldBe(notification.HouseholdId);
        publishedNotification.Type.ShouldBe(notification.Type);
        publishedNotification.Title.ShouldBe(notification.Title);
        publishedNotification.Message.ShouldBe(notification.Message);
        publishedNotification.OccurredAt.ShouldBe(notification.OccurredAt);
    }

    [Test]
    public async Task Consume_Should_NotPublishNotification_WhenNotificationDocumentIsMissing()
    {
        // Arrange
        var message = TestData.CreateHouseholdInvitationEmailFailedEvent();

        // Act
        await _harness.Bus.Publish(message);

        // Assert
        (await _harness.Consumed.Any<HouseholdInvitationEmailFailedEvent>()).ShouldBeTrue();
        _notificationPublisher.Notifications.ShouldBeEmpty();
    }
}
