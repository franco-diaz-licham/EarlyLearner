namespace EarlyLearner.Domain.IdentityContext.ValueObjects;

/// <summary>
/// Identifies a child profile inside a household.
/// </summary>
public readonly record struct ChildId(Guid Value);
