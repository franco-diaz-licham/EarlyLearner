using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;

namespace EarlyLearner.Domain.IdentityContext.Entities;

/// <summary>
/// Represents a preschool child whose learning journey is managed inside a
/// household. The child profile is owned by the household aggregate in the MVP.
/// </summary>
public sealed class Child : Entity<ChildId>
{
    private Child() { }

    internal Child(ChildId id, HouseholdId householdId, string givenName, DateOnly dateOfBirth)
    {
        Id = id;
        HouseholdId = householdId;
        GivenName = Required(givenName, nameof(givenName));
        DateOfBirth = dateOfBirth;
        IsArchived = false;
        SetCreatedOn();
    }

    /// <summary>
    /// Household this child profile belongs to.
    /// </summary>
    public HouseholdId HouseholdId { get; }

    /// <summary>
    /// Name used by carers when planning and recording this child's learning.
    /// </summary>
    public string GivenName { get; private set; } = default!;

    /// <summary>
    /// Birth date used for age-aware planning, readiness interpretation, and future activity suggestions.
    /// </summary>
    public DateOnly DateOfBirth { get; }

    /// <summary>
    /// Indicates the child profile is no longer actively managed while preserving historical records.
    /// </summary>
    public bool IsArchived { get; private set; }

    internal void Rename(string givenName)
    {
        GivenName = Required(givenName, nameof(givenName));
        SetUpdatedOn();
    }

    internal void Archive()
    {
        IsArchived = true;
        SetUpdatedOn();
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }


    #region Nav props
    public Household Household { get; private set; } = null!;
    #endregion
}
