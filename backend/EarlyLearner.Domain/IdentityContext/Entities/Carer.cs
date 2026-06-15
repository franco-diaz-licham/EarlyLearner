using EarlyLearner.Domain.Common;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Domain.IdentityContext.Entities;

/// <summary>
/// Represents a parent or caregiver inside a household. The carer is linked to
/// an ASP.NET Core Identity account by UserId and stores enough profile detail
/// for household screens and authorization decisions.
/// </summary>
public sealed class Carer : Entity<CarerId>
{
    internal Carer(CarerId id, UserId userId, string firstName, string lastName, HouseholdRoleEnum role)
        : base(id)
    {
        UserId = userId;
        FirstName = Required(firstName, nameof(firstName));
        LastName = Required(lastName, nameof(lastName));
        Role = role;
    }

    /// <summary>
    /// Authentication account linked to this carer profile.
    /// </summary>
    public UserId UserId { get; }

    /// <summary>
    /// Given name shown in household and parent-facing screens.
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    /// Family name shown in household and parent-facing screens.
    /// </summary>
    public string LastName { get; private set; }

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
    }

    internal void ChangeRole(HouseholdRoleEnum role)
    {
        Role = role;
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
