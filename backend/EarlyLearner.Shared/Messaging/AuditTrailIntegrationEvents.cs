namespace EarlyLearner.Shared.Messaging;

public sealed record AuditTrailEntryRecordedEvent(
    Guid Id,
    Guid HouseholdId,
    string Action,
    string Summary,
    string? Details,
    DateTimeOffset ActionedAt,
    DateTimeOffset OccurredAt) : IIntegrationEvent;
