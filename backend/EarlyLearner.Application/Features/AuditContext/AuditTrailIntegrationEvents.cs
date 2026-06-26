using EarlyLearner.Application.Ports;

namespace EarlyLearner.Application.Features.AuditContext;

public sealed record AuditTrailRecordRequested(
    Guid Id,
    Guid HouseholdId,
    string Action,
    string Summary,
    string? Details,
    DateTimeOffset ActionedAt,
    DateTimeOffset OccurredAt) : IIntegrationEvent;
