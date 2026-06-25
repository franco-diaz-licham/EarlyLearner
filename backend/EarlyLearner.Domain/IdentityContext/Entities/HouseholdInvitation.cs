using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Domain.IdentityContext.Entities;

public sealed class HouseholdInvitation : Entity<HouseholdInvitationId>
{
    public HouseholdId HouseholdId { get; private set; }
    public Household Household { get; private set; } = null!;
    public string Email { get; private set; } = default!;
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public HouseholdRoleEnum Role { get; private set; }
    public UserId InvitedByUserId { get; private set; }
    public User InvitedByUser { get; private set; } = null!;
    public DateTimeOffset InvitedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public HouseholdInvitationStatusEnum Status { get; private set; }
    public DateTimeOffset? AcceptedAt { get; private set; }
    public UserId? AcceptedByUserId { get; private set; }

    private HouseholdInvitation() { }

    private HouseholdInvitation(
        HouseholdInvitationId id,
        HouseholdId householdId,
        string email,
        string? firstName,
        string? lastName,
        HouseholdRoleEnum role,
        UserId invitedByUserId,
        DateTimeOffset expiresAt)
    {
        Id = id;
        HouseholdId = householdId;
        Email = Required(email, nameof(email)).ToLowerInvariant();
        FirstName = Optional(firstName);
        LastName = Optional(lastName);
        Role = role;
        InvitedByUserId = invitedByUserId;
        InvitedAt = DateTimeOffset.UtcNow;
        ExpiresAt = expiresAt;
        Status = HouseholdInvitationStatusEnum.Pending;
        SetCreatedOn();
    }

    internal static HouseholdInvitation CreatePending(
        HouseholdId householdId,
        string email,
        HouseholdRoleEnum role,
        UserId invitedByUserId,
        DateTimeOffset expiresAt)
    {
        return new HouseholdInvitation(
            new HouseholdInvitationId(Guid.NewGuid()),
            householdId,
            email,
            firstName: null,
            lastName: null,
            role,
            invitedByUserId,
            expiresAt);
    }

    public bool IsExpired(DateTimeOffset utcNow) => Status == HouseholdInvitationStatusEnum.Pending && ExpiresAt <= utcNow;

    public void Resend(DateTimeOffset expiresAt)
    {
        if (Status == HouseholdInvitationStatusEnum.Accepted) throw new DomainException("Accepted invitations cannot be resent.");

        Status = HouseholdInvitationStatusEnum.Pending;
        ExpiresAt = expiresAt;
        SetUpdatedOn();
    }

    public void Revoke()
    {
        if (Status == HouseholdInvitationStatusEnum.Accepted) throw new DomainException("Accepted invitations cannot be revoked.");

        Status = HouseholdInvitationStatusEnum.Revoked;
        SetUpdatedOn();
    }

    public void MarkAccepted(UserId acceptedByUserId)
    {
        if (Status != HouseholdInvitationStatusEnum.Pending) throw new DomainException("Only pending invitations can be accepted.");
        if (IsExpired(DateTimeOffset.UtcNow)) throw new DomainException("Expired invitations cannot be accepted.");

        AcceptedByUserId = acceptedByUserId;
        AcceptedAt = DateTimeOffset.UtcNow;
        Status = HouseholdInvitationStatusEnum.Accepted;
        SetUpdatedOn();
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }

    private static string? Optional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
