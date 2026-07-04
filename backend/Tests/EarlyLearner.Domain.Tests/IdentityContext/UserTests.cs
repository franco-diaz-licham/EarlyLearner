using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using Shouldly;

namespace EarlyLearner.Domain.Tests.IdentityContext;

[TestFixture]
public sealed class UserTests
{
    [Test]
    public void CreateActiveParent_ShouldNormalizeEmailAndSetProfileDetails_WhenValuesHaveWhitespaceAndMixedCase()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());

        // Act
        var user = User.CreateActiveParent(userId, " Parent@Example.COM ", " Avery ", " Taylor ");

        // Assert
        user.Id.ShouldBe(userId);
        user.Email.ShouldBe("parent@example.com");
        user.FirstName.ShouldBe("Avery");
        user.LastName.ShouldBe("Taylor");
        user.DisplayName.ShouldBe("Avery Taylor");
        user.Status.ShouldBe(UserAccountStatusEnum.Active);
        user.CreatedOn.ShouldNotBe(default);
    }

    [Test]
    public void CreatePendingParent_ShouldStartPending()
    {
        // Arrange
        const string email = "pending@example.com";

        // Act
        var user = User.CreatePendingParent(email, "Jordan", "Taylor");

        // Assert
        user.Status.ShouldBe(UserAccountStatusEnum.Pending);
        user.Email.ShouldBe(email);
    }

    [Test]
    public void LinkExternalIdentity_ShouldTrimExternalValuesAndActivateUser_WhenUserIsPending()
    {
        // Arrange
        var user = User.CreatePendingParent("pending@example.com", "Jordan", "Taylor");

        // Act
        user.LinkExternalIdentity(" external-object-id ", " tenant-id ");

        // Assert
        user.ExternalObjectId.ShouldBe("external-object-id");
        user.ExternalTenantId.ShouldBe("tenant-id");
        user.Status.ShouldBe(UserAccountStatusEnum.Active);
        user.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void UpdateProfile_ShouldNormalizeEmailAndTrimNames_WhenValuesHaveWhitespaceAndMixedCase()
    {
        // Arrange
        var user = User.CreateActiveParent(new UserId(Guid.NewGuid()), "old@example.com", "Old", "Name");

        // Act
        user.UpdateProfile(" New@Example.COM ", " Avery ", " Taylor ");

        // Assert
        user.Email.ShouldBe("new@example.com");
        user.FirstName.ShouldBe("Avery");
        user.LastName.ShouldBe("Taylor");
        user.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void Disable_ShouldMarkUserDisabled()
    {
        // Arrange
        var user = User.CreateActiveParent(new UserId(Guid.NewGuid()), "parent@example.com", "Avery", "Taylor");

        // Act
        user.Disable();

        // Assert
        user.Status.ShouldBe(UserAccountStatusEnum.Disabled);
        user.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void CreateActiveParent_ShouldThrow_WhenEmailIsMissing()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());

        // Act
        var exception = Should.Throw<DomainException>(() => User.CreateActiveParent(userId, " ", "Avery", "Taylor"));

        // Assert
        exception.Message.ShouldBe("email is required.");
    }
}
