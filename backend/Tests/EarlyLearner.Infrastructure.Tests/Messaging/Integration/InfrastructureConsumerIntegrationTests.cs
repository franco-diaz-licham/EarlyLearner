using EarlyLearner.Application.Ports;
using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using MassTransit;
using Moq;
using Shouldly;

namespace EarlyLearner.Infrastructure.Tests.Messaging.Integration;

[TestFixture]
public sealed class InfrastructureConsumerIntegrationTests : InfrastructureConsumerTestHostFixture
{
    [Test]
    public async Task HouseholdInvitationEmailSentEvent_Should_BeConsumedAndPublishNotification()
    {
        // Arrange
        var message = CreateSentEvent();
        var notification = CreateNotification(message.HouseholdId, message.InvitationId, NotificationDeliveryStatus.Succeeded);
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
        var message = CreateFailedEvent();
        var notification = CreateNotification(message.HouseholdId, message.InvitationId, NotificationDeliveryStatus.Failed);
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

    private static HouseholdInvitationEmailSentEvent CreateSentEvent()
    {
        return new HouseholdInvitationEmailSentEvent(
            Id: Guid.NewGuid(),
            HouseholdId: Guid.NewGuid(),
            InvitationId: Guid.NewGuid(),
            Email: "carer@example.com",
            SentAt: DateTimeOffset.UtcNow,
            OccurredAt: DateTimeOffset.UtcNow);
    }

    private static HouseholdInvitationEmailFailedEvent CreateFailedEvent()
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

    private static NotificationDocument CreateNotification(Guid householdId, Guid invitationId, NotificationDeliveryStatus status)
    {
        return new NotificationDocument(
            Id: NotificationDocument.BuildId(invitationId),
            HouseholdId: householdId,
            InvitationId: invitationId,
            Type: status == NotificationDeliveryStatus.Succeeded ? "householdInvitationEmailSent" : "householdInvitationEmailFailed",
            Title: status == NotificationDeliveryStatus.Succeeded ? "Invitation email sent" : "Invitation email failed",
            Message: status == NotificationDeliveryStatus.Succeeded ? "Invitation email was sent." : "Invitation email failed.",
            Status: status,
            OccurredAt: DateTimeOffset.UtcNow);
    }
}

