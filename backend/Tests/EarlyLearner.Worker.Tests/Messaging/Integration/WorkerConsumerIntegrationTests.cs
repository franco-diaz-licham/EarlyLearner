using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using EarlyLearner.Worker.Messaging;
using EarlyLearner.Worker.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace EarlyLearner.Worker.Tests.Messaging.Integration;

[TestFixture]
public sealed class WorkerConsumerIntegrationTests : WorkerConsumerTestHostFixture
{
    [Test]
    public async Task HouseholdInvitationEmailRequestedEvent_Should_BeConsumedAndPublishSentEvent()
    {
        // Arrange
        var message = CreateEmailRequestedEvent();
        _emailSender
            .Setup(sender => sender.SendAsync(It.Is<EmailMessageModel>(email => email.To == message.Email), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _harness.Bus.Publish(message);

        // Assert
        (await _harness.Consumed.Any<HouseholdInvitationEmailRequestedEvent>()).ShouldBeTrue();
        (await _harness.Published.Any<HouseholdInvitationEmailSentEvent>()).ShouldBeTrue();
        (await _harness.Published.Any<HouseholdInvitationEmailFailedEvent>()).ShouldBeFalse();

        var notification = _documentStore.GetNotification(message.HouseholdId, message.InvitationId);
        notification.ShouldNotBeNull();
        notification.Type.ShouldBe("householdInvitationEmailSent");
        notification.Status.ShouldBe(NotificationDeliveryStatus.Succeeded);
        _emailSender.Verify(sender => sender.SendAsync(It.IsAny<EmailMessageModel>(), It.IsAny<CancellationToken>()), Times.Once);
        _emailSender.VerifyNoOtherCalls();
    }

    [Test]
    public async Task HouseholdInvitationEmailRequestedEvent_Should_BeConsumedAndPublishFailedEvent_WhenEmailSenderThrows()
    {
        // Arrange
        var message = CreateEmailRequestedEvent();
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
        _emailSender.Verify(sender => sender.SendAsync(It.IsAny<EmailMessageModel>(), It.IsAny<CancellationToken>()), Times.Once);
        _emailSender.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AuditTrailEntryRecordedEvent_Should_BeConsumedAndStored()
    {
        // Arrange
        var message = CreateAuditTrailEntryRecordedEvent();

        // Act
        await _harness.Bus.Publish(message);

        // Assert
        (await _harness.Consumed.Any<AuditTrailEntryRecordedEvent>()).ShouldBeTrue();

        var db = ResolveService<AuditDbContext>();
        var entry = await db.AuditTrailEntries.SingleAsync();
        entry.Id.ShouldBe(message.Id);
        entry.HouseholdId.ShouldBe(message.HouseholdId);
        entry.Action.ShouldBe(message.Action);
        entry.Summary.ShouldBe(message.Summary);
        entry.Details.ShouldBe(message.Details);
        entry.ActionedAt.ShouldBe(message.ActionedAt);
    }

    private static HouseholdInvitationEmailRequestedEvent CreateEmailRequestedEvent()
    {
        return new HouseholdInvitationEmailRequestedEvent(
            Id: Guid.NewGuid(),
            HouseholdId: Guid.NewGuid(),
            InvitationId: Guid.NewGuid(),
            HouseholdName: "Early Learner Household",
            Email: "carer@example.com",
            FirstName: "Casey",
            LastName: "Carer",
            ExpiresAt: DateTimeOffset.UtcNow.AddDays(7),
            OccurredAt: DateTimeOffset.UtcNow);
    }

    private static AuditTrailEntryRecordedEvent CreateAuditTrailEntryRecordedEvent()
    {
        return new AuditTrailEntryRecordedEvent(
            Id: Guid.NewGuid(),
            HouseholdId: Guid.NewGuid(),
            Action: "HouseholdCarerInvited",
            Summary: "A household invitation was created.",
            Details: "carer@example.com",
            ActionedAt: DateTimeOffset.UtcNow.AddMinutes(-1),
            OccurredAt: DateTimeOffset.UtcNow);
    }
}
