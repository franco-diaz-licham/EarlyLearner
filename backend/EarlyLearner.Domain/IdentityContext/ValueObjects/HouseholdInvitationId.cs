namespace EarlyLearner.Domain.IdentityContext.ValueObjects;

/// <summary>
/// Identifies an invitation to join a household.
/// </summary>
public readonly record struct HouseholdInvitationId(Guid Value);
