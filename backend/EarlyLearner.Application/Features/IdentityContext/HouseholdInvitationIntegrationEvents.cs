using EarlyLearner.Application.Ports;

namespace EarlyLearner.Application.Features.IdentityContext;

public sealed record HouseholdInvitationEmailRequested(
    Guid Id,
    Guid HouseholdId,
    Guid InvitationId,
    string HouseholdName,
    string Email,
    string FirstName,
    string LastName,
    DateTimeOffset ExpiresAt,
    DateTimeOffset OccurredAt) : IIntegrationEvent;

public sealed record HouseholdInvitationNotificationRequested(
    Guid Id,
    Guid HouseholdId,
    Guid InvitationId,
    string HouseholdName,
    string Email,
    DateTimeOffset OccurredAt) : IIntegrationEvent;
