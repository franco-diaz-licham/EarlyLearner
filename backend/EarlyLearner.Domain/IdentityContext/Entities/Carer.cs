using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Domain.IdentityContext.Entities;

/// <summary>
/// Represents a user's membership inside a household.
/// </summary>
public sealed class Carer : Entity<CarerId>
{
    private Carer() { }

    internal Carer(CarerId id, HouseholdId householdId, UserId userId, HouseholdRoleEnum role)
    {
        Id = id;
        HouseholdId = householdId;
        UserId = userId;
        Role = role;
        SetCreatedOn();
    }

    /// <summary>
    /// Household this carer belongs to.
    /// </summary>
    public HouseholdId HouseholdId { get; }

    /// <summary>
    /// Household navigation for persistence and graph loading.
    /// </summary>
    public Household Household { get; private set; } = null!;

    /// <summary>
    /// Authentication account linked to this carer profile.
    /// </summary>
    public UserId UserId { get; }

    /// <summary>
    /// User navigation for profile and account lifecycle information.
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Household permission level for this carer.
    /// </summary>
    public HouseholdRoleEnum Role { get; private set; }

    internal void ChangeRole(HouseholdRoleEnum role)
    {
        Role = role;
        SetUpdatedOn();
    }
}
