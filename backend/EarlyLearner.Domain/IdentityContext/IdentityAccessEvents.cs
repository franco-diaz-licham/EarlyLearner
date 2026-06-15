using EarlyLearner.Domain.Common;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Domain.IdentityContext;

public sealed record ChildCreated(HouseholdId HouseholdId, ChildId ChildId, DateTimeOffset OccurredAt) : IDomainEvent;
