using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.Common;

namespace EarlyLearner.Domain.IdentityContext.Entities;

/// <summary>
/// Aggregate root that owns family membership and child profiles. It is the
/// primary ownership boundary used to protect sensitive child learning data.
/// </summary>
public sealed class Household : Entity<HouseholdId>
{
    private readonly List<Carer> _carers = [];
    private readonly List<Child> _children = [];

    private Household(HouseholdId id, string name) : base(id)
    {
        Name = Required(name, nameof(name));
    }

    /// <summary>
    /// Family-facing name used to identify the household in parent workflows.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Adults who can access or manage the household according to their assigned role.
    /// </summary>
    public IReadOnlyCollection<Carer> Carers => _carers.AsReadOnly();

    /// <summary>
    /// Child profiles whose learning records are owned by this household.
    /// </summary>
    public IReadOnlyCollection<Child> Children => _children.AsReadOnly();

    public static Household Create(string name, UserId ownerUserId, string ownerFirstName, string ownerLastName)
    {
        var household = new Household(new HouseholdId(Guid.NewGuid()), name);
        var owner = new Carer(new CarerId(Guid.NewGuid()), ownerUserId, ownerFirstName, ownerLastName, HouseholdRoleEnum.Owner);
        household._carers.Add(owner);
        return household;
    }

    public void AddCarer(UserId userId, string firstName, string lastName, HouseholdRoleEnum role)
    {
        if (_carers.Any(carer => carer.UserId == userId)) throw new DomainException("Carer already belongs to this household.");
        var carer = new Carer(new CarerId(Guid.NewGuid()), userId, firstName, lastName, role);
        _carers.Add(carer);
    }

    public Child AddChild(string givenName, DateOnly dateOfBirth)
    {
        var child = new Child(new ChildId(Guid.NewGuid()), givenName, dateOfBirth);
        _children.Add(child);
        RaiseDomainEvent(new ChildCreated(Id, child.Id, DateTimeOffset.UtcNow));
        return child;
    }

    public void ArchiveChild(ChildId childId)
    {
        var child = _children.SingleOrDefault(existingChild => existingChild.Id == childId);
        if (child is null) throw new DomainException("Child does not belong to this household.");

        child.Archive();
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
