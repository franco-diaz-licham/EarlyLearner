using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.Common;

namespace EarlyLearner.Domain.IdentityContext.Entities;

/// <summary>
/// Represents a preschool child whose learning journey is managed inside a
/// household. The child profile is owned by the household aggregate in the MVP.
/// </summary>
public sealed class Child : Entity<ChildId>
{
    internal Child(ChildId id, string givenName, DateOnly dateOfBirth) : base(id)
    {
        GivenName = Required(givenName, nameof(givenName));
        DateOfBirth = dateOfBirth;
        IsArchived = false;
    }

    /// <summary>
    /// Name used by carers when planning and recording this child's learning.
    /// </summary>
    public string GivenName { get; private set; }

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
    }

    internal void Archive()
    {
        IsArchived = true;
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
