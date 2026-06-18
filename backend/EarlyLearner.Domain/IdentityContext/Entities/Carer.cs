using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Domain.IdentityContext.Entities;

/// <summary>
/// Represents a parent or caregiver inside a household. The carer is linked to
/// an ASP.NET Core Identity account by UserId and stores enough profile detail
/// for household screens and authorization decisions.
/// </summary>
public sealed class Carer : Entity<CarerId>
{
    private Carer() { }

    internal Carer(CarerId id, HouseholdId householdId, UserId userId, string firstName, string lastName, HouseholdRoleEnum role)
    {
        Id = id;
        HouseholdId = householdId;
        UserId = userId;
        FirstName = Required(firstName, nameof(firstName));
        LastName = Required(lastName, nameof(lastName));
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
    /// Given name shown in household and parent-facing screens.
    /// </summary>
    public string FirstName { get; private set; } = default!;

    /// <summary>
    /// Family name shown in household and parent-facing screens.
    /// </summary>
    public string LastName { get; private set; } = default!;

    /// <summary>
    /// Household permission level for this carer.
    /// </summary>
    public HouseholdRoleEnum Role { get; private set; }

    /// <summary>
    /// Full name used when identifying the carer in the product experience.
    /// </summary>
    public string DisplayName => $"{FirstName} {LastName}";

    internal void Rename(string firstName, string lastName)
    {
        FirstName = Required(firstName, nameof(firstName));
        LastName = Required(lastName, nameof(lastName));
        SetUpdatedOn();
    }

    internal void ChangeRole(HouseholdRoleEnum role)
    {
        Role = role;
        SetUpdatedOn();
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
