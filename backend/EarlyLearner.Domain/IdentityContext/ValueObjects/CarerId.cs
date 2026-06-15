namespace EarlyLearner.Domain.IdentityContext.ValueObjects;

/// <summary>
/// Identifies a parent or caregiver profile inside a household.
/// </summary>
public readonly record struct CarerId(Guid Value);
