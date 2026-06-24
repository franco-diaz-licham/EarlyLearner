using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Domain.IdentityContext;

public sealed record ChildCreated(HouseholdId HouseholdId, ChildId ChildId, DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record HouseholdCarerInvited(
    HouseholdId HouseholdId,
    HouseholdInvitationId InvitationId,
    string HouseholdName,
    string Email,
    string? FirstName,
    string? LastName,
    DateTimeOffset ExpiresAt,
    DateTimeOffset OccurredAt) : IDomainEvent;
