using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using EarlyLearner.Shared.Tests.Fixtures;
using EarlyLearner.Worker.Messaging;
using Moq;
using Shouldly;

namespace EarlyLearner.Worker.Tests.Messaging.Consumers;

[TestFixture]
public sealed class HouseholdInvitationEmailRequestedConsumerIntegrationTests : WorkerConsumerIntegrationTestFixture
{
    [Test]
    public async Task Consume_Should_SendEmailUpsertNotificationAndPublishSentEvent()
    {
        // Arrange
        var message = TestData.CreateHouseholdInvitationEmailRequestedEvent();

        _emailSender
            .Setup(sender => sender.SendAsync(
                It.Is<EmailMessageModel>(email => email.To == message.Email),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _harness.Bus.Publish(message);

        // Assert
        (await _harness.Consumed.Any<HouseholdInvitationEmailRequestedEvent>()).ShouldBeTrue();
        (await _harness.Published.Any<HouseholdInvitationEmailSentEvent>()).ShouldBeTrue();
        (await _harness.Published.Any<HouseholdInvitationEmailFailedEvent>()).ShouldBeFalse();

        var notification = _documentStore.GetNotification(message.HouseholdId, message.InvitationId);
        notification.ShouldNotBeNull();
        notification.Id.ShouldBe(NotificationDocument.BuildId(message.InvitationId));
        notification.HouseholdId.ShouldBe(message.HouseholdId);
        notification.InvitationId.ShouldBe(message.InvitationId);
        notification.Type.ShouldBe("householdInvitationEmailSent");
        notification.Status.ShouldBe(NotificationDeliveryStatus.Succeeded);
    }

    [Test]
    public async Task Consume_Should_UpsertNotificationAndPublishFailedEvent_WhenEmailSenderThrows()
    {
        // Arrange
        var message = TestData.CreateHouseholdInvitationEmailRequestedEvent();

        _emailSender
            .Setup(sender => sender.SendAsync(It.IsAny<EmailMessageModel>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Email service is unavailable."));

        // Act
        await _harness.Bus.Publish(message);

        // Assert
        (await _harness.Consumed.Any<HouseholdInvitationEmailRequestedEvent>()).ShouldBeTrue();
        (await _harness.Published.Any<HouseholdInvitationEmailFailedEvent>()).ShouldBeTrue();
        (await _harness.Published.Any<HouseholdInvitationEmailSentEvent>()).ShouldBeFalse();

        var notification = _documentStore.GetNotification(message.HouseholdId, message.InvitationId);
        notification.ShouldNotBeNull();
        notification.Type.ShouldBe("householdInvitationEmailFailed");
        notification.Status.ShouldBe(NotificationDeliveryStatus.Failed);
        notification.Message.ShouldContain("Email service is unavailable.");
    }
}
