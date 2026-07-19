using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using Shouldly;

namespace EarlyLearner.Domain.Tests.IdentityContext;

[TestFixture]
public sealed class HouseholdTests
{
    private static readonly UserId OwnerUserId = new(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
    private static readonly UserId SecondUserId = new(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

    [Test]
    public void Create_ShouldTrimNameAndAddOwnerCarer_WhenNameHasWhitespace()
    {
        // Arrange
        const string householdName = " Taylor Household ";

        // Act
        var household = Household.Create(householdName, OwnerUserId);

        // Assert
        household.Id.Value.ShouldNotBe(Guid.Empty);
        household.Name.ShouldBe("Taylor Household");
        household.Carers.Count.ShouldBe(1);
        household.Carers.Single().UserId.ShouldBe(OwnerUserId);
        household.Carers.Single().Role.ShouldBe(HouseholdRoleEnum.Owner);
        household.CreatedOn.ShouldNotBe(default);
    }

    [Test]
    public void Rename_ShouldTrimNameAndSetUpdatedOn_WhenNameHasWhitespace()
    {
        // Arrange
        var household = CreateHousehold();

        // Act
        household.Rename(" New Household Name ");

        // Assert
        household.Name.ShouldBe("New Household Name");
        household.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void AcceptInvitation_ShouldAddNewHouseholdMemberAndMarkInvitationAccepted()
    {
        // Arrange
        var household = CreateHousehold();
        var invitation = household.InviteNewCarer("caregiver@example.com", HouseholdRoleEnum.Caregiver, OwnerUserId, DateTimeOffset.UtcNow.AddDays(7));
        household.ClearDomainEvents();

        // Act
        var carer = household.AcceptInvitation(invitation.Id, SecondUserId);

        // Assert
        invitation.Status.ShouldBe(HouseholdInvitationStatusEnum.Accepted);
        invitation.AcceptedByUserId.ShouldBe(SecondUserId);
        household.Carers.Count.ShouldBe(2);
        household.Carers.ShouldContain(carer);
        carer.UserId.ShouldBe(SecondUserId);
        carer.Role.ShouldBe(HouseholdRoleEnum.Caregiver);
        household.DomainEvents.OfType<EntityTraceRecordedEvent>().ShouldContain(trace => trace.Action == "HouseholdCarerAdded");
        household.DomainEvents.OfType<EntityTraceRecordedEvent>().ShouldContain(trace => trace.Action == "HouseholdCarerInvitationAccepted");
        household.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void AcceptInvitation_ShouldThrow_WhenUserAlreadyBelongsToHousehold()
    {
        // Arrange
        var household = CreateHousehold();
        var invitation = household.InviteNewCarer("owner@example.com", HouseholdRoleEnum.Caregiver, OwnerUserId, DateTimeOffset.UtcNow.AddDays(7));

        // Act
        var exception = Should.Throw<DomainException>(() => household.AcceptInvitation(invitation.Id, OwnerUserId));

        // Assert
        exception.Message.ShouldBe("Carer already belongs to this household.");
        invitation.Status.ShouldBe(HouseholdInvitationStatusEnum.Pending);
    }

    [Test]
    public void RemoveCarer_ShouldThrow_WhenRemovingOwner()
    {
        // Arrange
        var household = CreateHousehold();
        var ownerId = household.Carers.Single().Id;

        // Act
        var exception = Should.Throw<DomainException>(() => household.RemoveCarer(ownerId));

        // Assert
        exception.Message.ShouldBe("Household owner cannot be removed.");
    }

    [Test]
    public void RemoveCarer_ShouldRemoveNonOwnerCarer_WhenCarerBelongsToHousehold()
    {
        // Arrange
        var household = CreateHousehold();
        var invitation = household.InviteNewCarer("caregiver@example.com", HouseholdRoleEnum.Caregiver, OwnerUserId, DateTimeOffset.UtcNow.AddDays(7));
        var carer = household.AcceptInvitation(invitation.Id, SecondUserId);

        // Act
        household.RemoveCarer(carer.Id);

        // Assert
        household.Carers.ShouldNotContain(existingCarer => existingCarer.Id == carer.Id);
        household.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void InviteNewCarer_ShouldNormalizeEmailCreatePendingInvitationAndRaiseEvents_WhenEmailHasWhitespaceAndMixedCase()
    {
        // Arrange
        var household = CreateHousehold();
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);

        // Act
        var invitation = household.InviteNewCarer(" CAREGIVER@Example.COM ", HouseholdRoleEnum.Caregiver, OwnerUserId, expiresAt);

        // Assert
        invitation.Email.ShouldBe("caregiver@example.com");
        invitation.Status.ShouldBe(HouseholdInvitationStatusEnum.Pending);
        invitation.ExpiresAt.ShouldBe(expiresAt);
        household.Invitations.ShouldContain(invitation);
        household.DomainEvents.OfType<HouseholdCarerInvitedEvent>().Single().InvitationId.ShouldBe(invitation.Id);
        household.DomainEvents.OfType<EntityTraceRecordedEvent>().Single().Action.ShouldBe("HouseholdCarerInvited");
    }

    [Test]
    public void InviteExistingCarer_ShouldResendExistingPendingInvitation_WhenEmailMatchesPendingInvitation()
    {
        // Arrange
        var household = CreateHousehold();
        var original = household.InviteExistingCarer("caregiver@example.com", HouseholdRoleEnum.Caregiver, OwnerUserId, DateTimeOffset.UtcNow.AddDays(3));
        household.ClearDomainEvents();
        var newExpiry = DateTimeOffset.UtcNow.AddDays(10);

        // Act
        var resent = household.InviteExistingCarer("CAREGIVER@example.com", HouseholdRoleEnum.Caregiver, OwnerUserId, newExpiry);

        // Assert
        resent.ShouldBeSameAs(original);
        household.Invitations.Count.ShouldBe(1);
        resent.ExpiresAt.ShouldBe(newExpiry);
        household.DomainEvents.OfType<HouseholdCarerInvitedEvent>().Single().InvitationId.ShouldBe(original.Id);
    }

    [Test]
    public void AddChild_ShouldCreateChildAndRaiseEvents_WhenDetailsAreValid()
    {
        // Arrange
        var household = CreateHousehold();
        var avatarStoredFileId = new StoredFileId(Guid.NewGuid());
        var dateOfBirth = new DateOnly(2021, 3, 14);

        // Act
        var child = household.AddChild(" Mia ", " Taylor ", dateOfBirth, avatarStoredFileId);

        // Assert
        child.FirstName.ShouldBe("Mia");
        child.LastName.ShouldBe("Taylor");
        child.DateOfBirth.ShouldBe(dateOfBirth);
        child.AvatarStoredFileId.ShouldBe(avatarStoredFileId);
        household.Children.ShouldContain(child);
        household.DomainEvents.OfType<ChildCreatedEvent>().Single().ChildId.ShouldBe(child.Id);
        household.DomainEvents.OfType<EntityTraceRecordedEvent>().Single().Action.ShouldBe("ChildCreated");
    }

    [Test]
    public void UpdateChild_ShouldUpdateExistingChildDetails_WhenChildBelongsToHousehold()
    {
        // Arrange
        var household = CreateHousehold();
        var child = household.AddChild("Mia", "Taylor", new DateOnly(2021, 3, 14), avatarStoredFileId: null);
        var avatarStoredFileId = new StoredFileId(Guid.NewGuid());

        // Act
        household.UpdateChild(child.Id, " Amelia ", " Smith ", new DateOnly(2020, 12, 25), avatarStoredFileId);

        // Assert
        child.FirstName.ShouldBe("Amelia");
        child.LastName.ShouldBe("Smith");
        child.DateOfBirth.ShouldBe(new DateOnly(2020, 12, 25));
        child.AvatarStoredFileId.ShouldBe(avatarStoredFileId);
        child.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void ArchiveChild_ShouldMarkExistingChildArchived_WhenChildBelongsToHousehold()
    {
        // Arrange
        var household = CreateHousehold();
        var child = household.AddChild("Mia", "Taylor", new DateOnly(2021, 3, 14), avatarStoredFileId: null);

        // Act
        household.ArchiveChild(child.Id);

        // Assert
        child.IsArchived.ShouldBeTrue();
        child.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void UpdateChild_ShouldThrow_WhenChildDoesNotBelongToHousehold()
    {
        // Arrange
        var household = CreateHousehold();
        var unknownChildId = new ChildId(Guid.NewGuid());

        // Act
        var exception = Should.Throw<DomainException>(() => household.UpdateChild(unknownChildId, "Mia", "Taylor", new DateOnly(2021, 3, 14), avatarStoredFileId: null));

        // Assert
        exception.Message.ShouldBe("Child does not belong to this household.");
    }

    [Test]
    public void AddChild_ShouldThrow_WhenFirstNameIsMissing()
    {
        // Arrange
        var household = CreateHousehold();

        // Act
        var exception = Should.Throw<DomainException>(() => household.AddChild(" ", "Taylor", new DateOnly(2021, 3, 14), avatarStoredFileId: null));

        // Assert
        exception.Message.ShouldBe("firstName is required.");
    }

    [Test]
    public void ArchiveChild_ShouldThrow_WhenChildDoesNotBelongToHousehold()
    {
        // Arrange
        var household = CreateHousehold();
        var unknownChildId = new ChildId(Guid.NewGuid());

        // Act
        var exception = Should.Throw<DomainException>(() => household.ArchiveChild(unknownChildId));

        // Assert
        exception.Message.ShouldBe("Child does not belong to this household.");
    }

    [Test]
    public void RevokeInvitation_ShouldRevokeExistingInvitationAndRaiseTraceEvent()
    {
        // Arrange
        var household = CreateHousehold();
        var invitation = household.InviteNewCarer("caregiver@example.com", HouseholdRoleEnum.Caregiver, OwnerUserId, DateTimeOffset.UtcNow.AddDays(7));
        household.ClearDomainEvents();

        // Act
        household.RevokeInvitation(invitation.Id);

        // Assert
        invitation.Status.ShouldBe(HouseholdInvitationStatusEnum.Revoked);
        invitation.UpdatedOn.ShouldNotBeNull();
        household.UpdatedOn.ShouldNotBeNull();
        household.DomainEvents.OfType<EntityTraceRecordedEvent>().Single().Action.ShouldBe("HouseholdCarerInvitationRevoked");
    }

    [Test]
    public void RevokeInvitation_ShouldThrow_WhenInvitationDoesNotBelongToHousehold()
    {
        // Arrange
        var household = CreateHousehold();
        var unknownInvitationId = new HouseholdInvitationId(Guid.NewGuid());

        // Act
        var exception = Should.Throw<DomainException>(() => household.RevokeInvitation(unknownInvitationId));

        // Assert
        exception.Message.ShouldBe("Invitation does not belong to this household.");
    }
    private static Household CreateHousehold()
    {
        return Household.Create("Taylor Household", OwnerUserId);
    }
}
