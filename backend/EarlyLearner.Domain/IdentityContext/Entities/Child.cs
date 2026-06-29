using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.ValueObjects;

namespace EarlyLearner.Domain.IdentityContext.Entities;

/// <summary>
/// Represents a preschool child whose learning journey is managed inside a
/// household. The child profile is owned by the household aggregate in the MVP.
/// </summary>
public sealed class Child : Entity<ChildId>
{
    private Child() { }

    internal Child(ChildId id, HouseholdId householdId, string firstName, string lastName, DateOnly dateOfBirth, StoredFileId? avatarStoredFileId)
    {
        Id = id;
        HouseholdId = householdId;
        FirstName = Required(firstName, nameof(firstName));
        LastName = Required(lastName, nameof(lastName));
        DateOfBirth = dateOfBirth;
        AvatarStoredFileId = avatarStoredFileId;
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
    public string FirstName { get; private set; } = default!;

    /// <summary>
    /// Family name shown in household and parent-facing screens.
    /// </summary>
    public string LastName { get; private set; } = default!;

    /// <summary>
    /// Birth date used for age-aware planning, readiness interpretation, and future activity suggestions.
    /// </summary>
    public DateOnly DateOfBirth { get; private set; }

    /// <summary>
    /// Optional stored file used as the child's profile avatar.
    /// </summary>
    public StoredFileId? AvatarStoredFileId { get; private set; }

    /// <summary>
    /// Indicates the child profile is no longer actively managed while preserving historical records.
    /// </summary>
    public bool IsArchived { get; private set; }

    internal void Rename(string firstName, string lastName)
    {
        FirstName = Required(firstName, nameof(firstName));
        LastName = Required(lastName, nameof(lastName));
        SetUpdatedOn();
    }

    internal void UpdateDetails(string firstName, string lastName, DateOnly dateOfBirth, StoredFileId? avatarStoredFileId)
    {
        FirstName = Required(firstName, nameof(firstName));
        LastName = Required(lastName, nameof(lastName));
        DateOfBirth = dateOfBirth;
        AvatarStoredFileId = avatarStoredFileId;
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
