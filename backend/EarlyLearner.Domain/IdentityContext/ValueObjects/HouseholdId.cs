namespace EarlyLearner.Domain.IdentityContext.ValueObjects;

/// <summary>
/// Identifies a household that owns parent and child learning data.
/// </summary>
public readonly record struct HouseholdId(Guid Value);
