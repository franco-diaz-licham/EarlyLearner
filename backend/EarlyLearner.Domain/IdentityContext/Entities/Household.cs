using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.ValueObjects;

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
        var previousName = Name;
        Name = Required(name, nameof(name));
        var occurredAt = DateTimeOffset.UtcNow;
        RaiseHouseholdTrace(
            entityId: Id.Value.ToString(),
            action: "HouseholdRenamed",
            summary: "Household renamed",
            details: $"Household was renamed from {previousName} to {Name}.",
            occurredAt: occurredAt);
        SetUpdatedOn();
    }

    public Carer AcceptInvitation(HouseholdInvitationId invitationId, UserId acceptedByUserId)
    {
        var invitation = _invitations.SingleOrDefault(existingInvitation => existingInvitation.Id == invitationId);
        if (invitation is null) throw new DomainException("Invitation does not belong to this household.");
        if (_carers.Any(carer => carer.UserId == acceptedByUserId)) throw new DomainException("Carer already belongs to this household.");

        invitation.MarkAccepted(acceptedByUserId);
        var carer = AddCarerMembership(acceptedByUserId, invitation.Role);
        var occurredAt = DateTimeOffset.UtcNow;
        RaiseTraceEvent(
            entityName: nameof(HouseholdInvitation),
            entityId: invitation.Id.Value.ToString(),
            action: "HouseholdCarerInvitationAccepted",
            summary: "Carer invitation accepted",
            details: $"{invitation.Email} accepted an invitation to join {Name}.",
            householdId: Id.Value,
            occurredAt: occurredAt);
        SetUpdatedOn();
        return carer;
    }

    private Carer AddCarerMembership(UserId userId, HouseholdRoleEnum role)
    {
        var carer = new Carer(new CarerId(Guid.NewGuid()), Id, userId, role);
        _carers.Add(carer);
        var occurredAt = DateTimeOffset.UtcNow;
        RaiseTraceEvent(
            entityName: nameof(Carer),
            entityId: carer.Id.Value.ToString(),
            action: "HouseholdCarerAdded",
            summary: "Carer added",
            details: $"Carer {userId.Value} was added to {Name} as {role}.",
            householdId: Id.Value,
            occurredAt: occurredAt);
        return carer;
    }

    public HouseholdInvitation InviteNewCarer(string email, HouseholdRoleEnum role, UserId invitedByUserId, DateTimeOffset expiresAt)
    {
        var normalizedEmail = Required(email, nameof(email)).ToLowerInvariant();
        var existingInvitation = GetLatestInvitation(normalizedEmail);

        if (existingInvitation is not null && existingInvitation.Status == HouseholdInvitationStatusEnum.Pending) {
            existingInvitation.Resend(expiresAt);
            RaiseCarerInvited(existingInvitation);
            SetUpdatedOn();
            return existingInvitation;
        }

        var invitation = HouseholdInvitation.CreatePending(Id, normalizedEmail, role, invitedByUserId, expiresAt);

        _invitations.Add(invitation);
        RaiseCarerInvited(invitation);
        SetUpdatedOn();
        return invitation;
    }

    public HouseholdInvitation InviteExistingCarer(string email, HouseholdRoleEnum role, UserId invitedByUserId, DateTimeOffset expiresAt)
    {
        var normalizedEmail = Required(email, nameof(email)).ToLowerInvariant();
        var existingInvitation = GetLatestInvitation(normalizedEmail);

        if (existingInvitation is not null && existingInvitation.Status == HouseholdInvitationStatusEnum.Pending) {
            existingInvitation.Resend(expiresAt);
            RaiseCarerInvited(existingInvitation);
            SetUpdatedOn();
            return existingInvitation;
        }

        var invitation = HouseholdInvitation.CreatePending(Id, normalizedEmail, role, invitedByUserId, expiresAt);

        _invitations.Add(invitation);
        RaiseCarerInvited(invitation);
        SetUpdatedOn();
        return invitation;
    }

    private HouseholdInvitation? GetLatestInvitation(string normalizedEmail)
    {
        return _invitations
            .Where(invitation => invitation.Email == normalizedEmail)
            .OrderByDescending(invitation => invitation.InvitedAt)
            .FirstOrDefault();
    }

    private void RaiseCarerInvited(HouseholdInvitation invitation)
    {
        var occurredAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new HouseholdCarerInvited(
            Id,
            invitation.Id,
            Name,
            invitation.Email,
            invitation.FirstName,
            invitation.LastName,
            invitation.ExpiresAt,
            occurredAt));

        RaiseTraceEvent(
            entityName: nameof(HouseholdInvitation),
            entityId: invitation.Id.Value.ToString(),
            action: "HouseholdCarerInvited",
            summary: "Carer invited",
            details: $"{invitation.Email} was invited to join {Name}.",
            householdId: Id.Value,
            occurredAt: occurredAt);
    }

    public void RemoveCarer(CarerId carerId)
    {
        var carer = _carers.SingleOrDefault(existingCarer => existingCarer.Id == carerId);
        if (carer is null) throw new DomainException("Carer does not belong to this household.");
        if (carer.Role == HouseholdRoleEnum.Owner) throw new DomainException("Household owner cannot be removed.");

        _carers.Remove(carer);
        var occurredAt = DateTimeOffset.UtcNow;
        RaiseTraceEvent(
            entityName: nameof(Carer),
            entityId: carer.Id.Value.ToString(),
            action: "HouseholdCarerRemoved",
            summary: "Carer removed",
            details: $"Carer {carer.UserId.Value} was removed from {Name}.",
            householdId: Id.Value,
            occurredAt: occurredAt);
        SetUpdatedOn();
    }

    public void RevokeInvitation(HouseholdInvitationId invitationId)
    {
        var invitation = _invitations.SingleOrDefault(existingInvitation => existingInvitation.Id == invitationId);
        if (invitation is null) throw new DomainException("Invitation does not belong to this household.");

        invitation.Revoke();
        var occurredAt = DateTimeOffset.UtcNow;
        RaiseTraceEvent(
            entityName: nameof(HouseholdInvitation),
            entityId: invitation.Id.Value.ToString(),
            action: "HouseholdCarerInvitationRevoked",
            summary: "Carer invitation revoked",
            details: $"{invitation.Email} can no longer join {Name} using this invitation.",
            householdId: Id.Value,
            occurredAt: occurredAt);
        SetUpdatedOn();
    }

    public Child AddChild(string firstName, string lastName, DateOnly dateOfBirth, StoredFileId? avatarStoredFileId)
    {
        var child = new Child(new ChildId(Guid.NewGuid()), Id, firstName, lastName, dateOfBirth, avatarStoredFileId);
        _children.Add(child);
        var occurredAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new ChildCreated(Id, child.Id, occurredAt));
        RaiseTraceEvent(
            entityName: nameof(Child),
            entityId: child.Id.Value.ToString(),
            action: "ChildCreated",
            summary: "Child profile created",
            details: $"Child profile {child.Id.Value} was created.",
            householdId: Id.Value,
            occurredAt: occurredAt);
        SetUpdatedOn();
        return child;
    }

    public void ArchiveChild(ChildId childId)
    {
        var child = _children.SingleOrDefault(existingChild => existingChild.Id == childId);
        if (child is null) throw new DomainException("Child does not belong to this household.");

        child.Archive();
        var occurredAt = DateTimeOffset.UtcNow;
        RaiseTraceEvent(
            entityName: nameof(Child),
            entityId: child.Id.Value.ToString(),
            action: "ChildArchived",
            summary: "Child profile archived",
            details: $"Child profile {child.Id.Value} was archived.",
            householdId: Id.Value,
            occurredAt: occurredAt);
        SetUpdatedOn();
    }

    public void UpdateChild(ChildId childId, string firstName, string lastName, DateOnly dateOfBirth, StoredFileId? avatarStoredFileId)
    {
        var child = _children.SingleOrDefault(existingChild => existingChild.Id == childId);
        if (child is null) throw new DomainException("Child does not belong to this household.");

        child.UpdateDetails(firstName, lastName, dateOfBirth, avatarStoredFileId);
        var occurredAt = DateTimeOffset.UtcNow;
        RaiseTraceEvent(
            entityName: nameof(Child),
            entityId: child.Id.Value.ToString(),
            action: "ChildUpdated",
            summary: "Child profile updated",
            details: $"Child profile {child.Id.Value} was updated.",
            householdId: Id.Value,
            occurredAt: occurredAt);
        SetUpdatedOn();
    }

    private void RaiseHouseholdTrace(string entityId, string action, string summary, string? details, DateTimeOffset occurredAt)
    {
        RaiseTraceEvent(
            entityName: nameof(Household),
            entityId: entityId,
            action: action,
            summary: summary,
            details: details,
            householdId: Id.Value,
            occurredAt: occurredAt);
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
