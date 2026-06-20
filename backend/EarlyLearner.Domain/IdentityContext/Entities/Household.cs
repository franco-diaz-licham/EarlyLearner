using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;

namespace EarlyLearner.Domain.IdentityContext.Entities;

/// <summary>
/// Aggregate root that owns family membership and child profiles. It is the
/// primary ownership boundary used to protect sensitive child learning data.
/// </summary>
public sealed class Household : Entity<HouseholdId>
{
    private readonly List<Carer> _carers = [];
    private readonly List<Child> _children = [];

    private Household() { }

    private Household(HouseholdId id, string name)
    {
        Id = id;
        Name = Required(name, nameof(name));
        SetCreatedOn();
    }

    /// <summary>
    /// Family-facing name used to identify the household in parent workflows.
    /// </summary>
    public string Name { get; private set; } = default!;

    #region Nav props

    /// <summary>
    /// Adults who can access or manage the household according to their assigned role.
    /// </summary>
    public IReadOnlyCollection<Carer> Carers => _carers.AsReadOnly();

    /// <summary>
    /// Child profiles whose learning records are owned by this household.
    /// </summary>
    public IReadOnlyCollection<Child> Children => _children.AsReadOnly();

    #endregion

    public static Household Create(string name, UserId ownerUserId, string ownerFirstName, string ownerLastName)
    {
        var household = new Household(new HouseholdId(Guid.NewGuid()), name);
        var owner = new Carer(new CarerId(Guid.NewGuid()), household.Id, ownerUserId, HouseholdRoleEnum.Owner);
        household._carers.Add(owner);
        return household;
    }

    public void Rename(string name)
    {
        Name = Required(name, nameof(name));
        SetUpdatedOn();
    }

    public void AddCarer(UserId userId, HouseholdRoleEnum role)
    {
        if (_carers.Any(carer => carer.UserId == userId)) throw new DomainException("Carer already belongs to this household.");
        var carer = new Carer(new CarerId(Guid.NewGuid()), Id, userId, role);
        _carers.Add(carer);
        SetUpdatedOn();
    }

    public void RemoveCarer(CarerId carerId)
    {
        var carer = _carers.SingleOrDefault(existingCarer => existingCarer.Id == carerId);
        if (carer is null) throw new DomainException("Carer does not belong to this household.");
        if (carer.Role == HouseholdRoleEnum.Owner) throw new DomainException("Household owner cannot be removed.");

        _carers.Remove(carer);
        SetUpdatedOn();
    }

    public Child AddChild(string givenName, DateOnly dateOfBirth)
    {
        var child = new Child(new ChildId(Guid.NewGuid()), Id, givenName, dateOfBirth);
        _children.Add(child);
        RaiseDomainEvent(new ChildCreated(Id, child.Id, DateTimeOffset.UtcNow));
        SetUpdatedOn();
        return child;
    }

    public void ArchiveChild(ChildId childId)
    {
        var child = _children.SingleOrDefault(existingChild => existingChild.Id == childId);
        if (child is null) throw new DomainException("Child does not belong to this household.");

        child.Archive();
        SetUpdatedOn();
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
