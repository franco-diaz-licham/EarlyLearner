using EarlyLearner.Application.Ports;

namespace EarlyLearner.Application.Features.IdentityContext;

public sealed record HouseholdInvitationEmailRequested(
    Guid Id,
    Guid HouseholdId,
    Guid InvitationId,
    string HouseholdName,
    string Email,
    string? FirstName,
    string? LastName,
    DateTimeOffset ExpiresAt,
    DateTimeOffset OccurredAt) : IIntegrationEvent;

public sealed record HouseholdInvitationEmailSent(
    Guid Id,
    Guid HouseholdId,
    Guid InvitationId,
    string Email,
    DateTimeOffset SentAt,
    DateTimeOffset OccurredAt) : IIntegrationEvent;

public sealed record HouseholdInvitationEmailFailed(
    Guid Id,
    Guid HouseholdId,
    Guid InvitationId,
    string Email,
    string Reason,
    DateTimeOffset FailedAt,
    DateTimeOffset OccurredAt) : IIntegrationEvent;
