using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using EarlyLearner.Shared.Tests.Fixtures;
using Shouldly;

namespace EarlyLearner.Infrastructure.Tests.Messaging.Consumers;

[TestFixture]
public sealed class HouseholdInvitationEmailSentConsumerIntegrationTests : InfrastructureConsumerIntegrationTestFixture
{
    [Test]
    public async Task Consume_Should_PublishNotification_WhenNotificationDocumentExists()
    {
        // Arrange
        var message = TestData.CreateHouseholdInvitationEmailSentEvent();
        var notification = TestData.CreateNotification(message.HouseholdId, message.InvitationId, NotificationDeliveryStatus.Succeeded);
        await _documentStore.UpsertAsync(NotificationDocument.ContainerName, notification, NotificationDocument.BuildPartitionKey(message.HouseholdId));

        // Act
        await _harness.Bus.Publish(message);

        // Assert
        (await _harness.Consumed.Any<HouseholdInvitationEmailSentEvent>()).ShouldBeTrue();

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
        var message = TestData.CreateHouseholdInvitationEmailSentEvent();

        // Act
        await _harness.Bus.Publish(message);

        // Assert
        (await _harness.Consumed.Any<HouseholdInvitationEmailSentEvent>()).ShouldBeTrue();
        _notificationPublisher.Notifications.ShouldBeEmpty();
    }
}
