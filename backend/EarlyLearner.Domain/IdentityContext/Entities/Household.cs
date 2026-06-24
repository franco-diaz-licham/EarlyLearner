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
    private readonly List<HouseholdInvitation> _invitations = [];

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

    /// <summary>
    /// Invitations for people who have been asked to join the household.
    /// </summary>
    public IReadOnlyCollection<HouseholdInvitation> Invitations => _invitations.AsReadOnly();

    #endregion

    public static Household Create(string name, UserId ownerUserId)
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

    public HouseholdInvitation InviteCarer(string email, HouseholdRoleEnum role, UserId invitedByUserId, DateTimeOffset expiresAt)
    {
        var normalizedEmail = Required(email, nameof(email)).ToLowerInvariant();
        var existingInvitation = _invitations
            .Where(invitation => invitation.Email == normalizedEmail)
            .OrderByDescending(invitation => invitation.InvitedAt)
            .FirstOrDefault();

        if (existingInvitation is not null && existingInvitation.Status == HouseholdInvitationStatusEnum.Pending) {
            existingInvitation.Resend(expiresAt);
            RaiseInvitationEvent(existingInvitation);
            SetUpdatedOn();
            return existingInvitation;
        }

        var invitation = HouseholdInvitation.CreatePending(
            Id,
            normalizedEmail,
            role,
            invitedByUserId,
            expiresAt);

        _invitations.Add(invitation);
        RaiseInvitationEvent(invitation);
        SetUpdatedOn();
        return invitation;
    }

    private void RaiseInvitationEvent(HouseholdInvitation invitation)
    {
        RaiseDomainEvent(new HouseholdCarerInvited(
            Id,
            invitation.Id,
            Name,
            invitation.Email,
            invitation.FirstName,
            invitation.LastName,
            invitation.ExpiresAt,
            DateTimeOffset.UtcNow));
    }

    public void RemoveCarer(CarerId carerId)
    {
        var carer = _carers.SingleOrDefault(existingCarer => existingCarer.Id == carerId);
        if (carer is null) throw new DomainException("Carer does not belong to this household.");
        if (carer.Role == HouseholdRoleEnum.Owner) throw new DomainException("Household owner cannot be removed.");

        _carers.Remove(carer);
        SetUpdatedOn();
    }

    public Child AddChild(string firstName, string lastName, DateOnly dateOfBirth)
    {
        var child = new Child(new ChildId(Guid.NewGuid()), Id, firstName, lastName, dateOfBirth);
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

    public void UpdateChild(ChildId childId, string firstName, string lastName, DateOnly dateOfBirth)
    {
        var child = _children.SingleOrDefault(existingChild => existingChild.Id == childId);
        if (child is null) throw new DomainException("Child does not belong to this household.");

        child.UpdateDetails(firstName, lastName, dateOfBirth);
        SetUpdatedOn();
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
