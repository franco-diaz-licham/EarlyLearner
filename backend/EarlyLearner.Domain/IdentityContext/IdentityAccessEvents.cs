using EarlyLearner.Domain.Common;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Domain.IdentityContext;

public sealed record HouseholdCreated(
    HouseholdId HouseholdId,
    CarerId OwnerCarerId,
    UserId OwnerUserId,
    string OwnerFirstName,
    string OwnerLastName,
    DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record CarerJoinedHousehold(
    HouseholdId HouseholdId,
    CarerId CarerId,
    UserId UserId,
    string FirstName,
    string LastName,
    HouseholdRoleEnum Role,
    DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record ChildCreated(HouseholdId HouseholdId, ChildId ChildId, DateTimeOffset OccurredAt) : IDomainEvent;
