namespace EarlyLearner.Worker.Application.AuditTrail;

public sealed record AuditTrailEntryModel(
    Guid Id,
    Guid HouseholdId,
    string Action,
    string Summary,
    string? Details,
    DateTimeOffset ActionedAt,
    DateTimeOffset RecordedAt);
