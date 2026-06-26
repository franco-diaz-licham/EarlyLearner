namespace EarlyLearner.Worker.Infrastructure.Persistence.Entities;

public sealed class AuditTrailEntry
{
    public Guid Id { get; init; }

    public Guid HouseholdId { get; init; }

    public string Action { get; init; } = string.Empty;

    public string Summary { get; init; } = string.Empty;

    public string? Details { get; init; }

    public DateTimeOffset ActionedAt { get; init; }

    public DateTimeOffset RecordedAt { get; init; }
}
