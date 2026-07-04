using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using Shouldly;

namespace EarlyLearner.Domain.Tests.IdentityContext;

[TestFixture]
public sealed class HouseholdInvitationTests
{
    private static readonly UserId InviterUserId = new(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
    private static readonly UserId AcceptedByUserId = new(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

    [Test]
    public void InviteNewCarer_ShouldCreatePendingInvitation()
    {
        // Arrange
        var household = Household.Create("Taylor Household", InviterUserId);
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);

        // Act
        var invitation = household.InviteNewCarer(" caregiver@example.com ", HouseholdRoleEnum.Caregiver, InviterUserId, expiresAt);

        // Assert
        invitation.HouseholdId.ShouldBe(household.Id);
        invitation.Email.ShouldBe("caregiver@example.com");
        invitation.Role.ShouldBe(HouseholdRoleEnum.Caregiver);
        invitation.InvitedByUserId.ShouldBe(InviterUserId);
        invitation.Status.ShouldBe(HouseholdInvitationStatusEnum.Pending);
        invitation.AcceptedAt.ShouldBeNull();
        invitation.AcceptedByUserId.ShouldBeNull();
    }

    [Test]
    public void IsExpired_ShouldReturnTrue_WhenPendingInvitationIsExpired()
    {
        // Arrange
        var invitation = CreateInvitation(DateTimeOffset.UtcNow.AddMinutes(-1));

        // Act
        var isExpired = invitation.IsExpired(DateTimeOffset.UtcNow);

        // Assert
        isExpired.ShouldBeTrue();
    }

    [Test]
    public void Resend_ShouldSetPendingStatusAndReplaceExpiry_WhenInvitationIsRevoked()
    {
        // Arrange
        var invitation = CreateInvitation(DateTimeOffset.UtcNow.AddDays(1));
        invitation.Revoke();
        var newExpiry = DateTimeOffset.UtcNow.AddDays(7);

        // Act
        invitation.Resend(newExpiry);

        // Assert
        invitation.Status.ShouldBe(HouseholdInvitationStatusEnum.Pending);
        invitation.ExpiresAt.ShouldBe(newExpiry);
        invitation.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void MarkAccepted_ShouldAcceptPendingInvitation_WhenInvitationIsPendingAndNotExpired()
    {
        // Arrange
        var invitation = CreateInvitation(DateTimeOffset.UtcNow.AddDays(1));

        // Act
        invitation.MarkAccepted(AcceptedByUserId);

        // Assert
        invitation.Status.ShouldBe(HouseholdInvitationStatusEnum.Accepted);
        invitation.AcceptedByUserId.ShouldBe(AcceptedByUserId);
        invitation.AcceptedAt.ShouldNotBeNull();
        invitation.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void MarkAccepted_ShouldThrow_WhenInvitationIsExpired()
    {
        // Arrange
        var invitation = CreateInvitation(DateTimeOffset.UtcNow.AddMinutes(-1));

        // Act
        var exception = Should.Throw<DomainException>(() => invitation.MarkAccepted(AcceptedByUserId));

        // Assert
        exception.Message.ShouldBe("Expired invitations cannot be accepted.");
    }

    [Test]
    public void MarkAccepted_ShouldThrow_WhenInvitationIsRevoked()
    {
        // Arrange
        var invitation = CreateInvitation(DateTimeOffset.UtcNow.AddDays(1));
        invitation.Revoke();

        // Act
        var exception = Should.Throw<DomainException>(() => invitation.MarkAccepted(AcceptedByUserId));

        // Assert
        exception.Message.ShouldBe("Only pending invitations can be accepted.");
    }

    [Test]
    public void Revoke_ShouldThrow_WhenInvitationIsAccepted()
    {
        // Arrange
        var invitation = CreateInvitation(DateTimeOffset.UtcNow.AddDays(1));
        invitation.MarkAccepted(AcceptedByUserId);

        // Act
        var exception = Should.Throw<DomainException>(invitation.Revoke);

        // Assert
        exception.Message.ShouldBe("Accepted invitations cannot be revoked.");
    }

    [Test]
    public void Resend_ShouldThrow_WhenInvitationIsAccepted()
    {
        // Arrange
        var invitation = CreateInvitation(DateTimeOffset.UtcNow.AddDays(1));
        invitation.MarkAccepted(AcceptedByUserId);

        // Act
        var exception = Should.Throw<DomainException>(() => invitation.Resend(DateTimeOffset.UtcNow.AddDays(7)));

        // Assert
        exception.Message.ShouldBe("Accepted invitations cannot be resent.");
    }

    private static HouseholdInvitation CreateInvitation(DateTimeOffset expiresAt)
    {
        var household = Household.Create("Taylor Household", InviterUserId);
        return household.InviteNewCarer("caregiver@example.com", HouseholdRoleEnum.Caregiver, InviterUserId, expiresAt);
    }
}
