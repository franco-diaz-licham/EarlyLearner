using EarlyLearner.Shared.Messaging;

namespace EarlyLearner.Shared.AuditContext;

public sealed record AuditTrailEntryRecordedEvent(
    Guid Id,
    Guid HouseholdId,
    string Action,
    string Summary,
    string? Details,
    DateTimeOffset ActionedAt,
    DateTimeOffset OccurredAt) : IIntegrationEvent;

public static class AuditMessagingTopology
{
    public const string AuditTrailEntryRecordedTopic = "audit-trail-entry-recorded";
    public const string AuditWorkerSubscription = "earlylearner.audit-worker";
}
