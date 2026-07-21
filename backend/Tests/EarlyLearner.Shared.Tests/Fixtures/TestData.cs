using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;

namespace EarlyLearner.Shared.Tests.Fixtures;

public static class TestData
{
    public static HouseholdInvitationEmailRequestedEvent CreateHouseholdInvitationEmailRequestedEvent()
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

    public static AuditTrailEntryRecordedEvent CreateAuditTrailEntryRecordedEvent()
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

    public static HouseholdInvitationEmailSentEvent CreateHouseholdInvitationEmailSentEvent()
    {
        return new HouseholdInvitationEmailSentEvent(
            Id: Guid.NewGuid(),
            HouseholdId: Guid.NewGuid(),
            InvitationId: Guid.NewGuid(),
            Email: "carer@example.com",
            SentAt: DateTimeOffset.UtcNow,
            OccurredAt: DateTimeOffset.UtcNow);
    }

    public static HouseholdInvitationEmailFailedEvent CreateHouseholdInvitationEmailFailedEvent()
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

    public static NotificationDocument CreateNotification(
        Guid householdId,
        Guid invitationId,
        NotificationDeliveryStatus status = NotificationDeliveryStatus.Succeeded)
    {
        return new NotificationDocument(
            Id: NotificationDocument.BuildId(invitationId),
            HouseholdId: householdId,
            InvitationId: invitationId,
            Type: "householdInvitationEmail",
            Title: "Household invitation email",
            Message: "Household invitation email status changed.",
            Status: status,
            OccurredAt: DateTimeOffset.UtcNow);
    }
}