using EarlyLearner.Application.Ports;

namespace EarlyLearner.Application.Features.AuditContext;

public sealed record AuditTrailEntryRecorded(
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
