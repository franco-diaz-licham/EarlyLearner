using EarlyLearner.Application.Common;
using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using Moq;
using Shouldly;

namespace EarlyLearner.Application.Tests.IdentityContext;

[TestFixture]
public class CurrentUserProvisioningServiceTests
{
    private Mock<IUserProvisioningRepository> _userRepo = default!;
    private Mock<IUnitOfWork> _uow = default!;
    private CurrentUserProvisioningService _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _userRepo = new Mock<IUserProvisioningRepository>(MockBehavior.Strict);
        _uow = new Mock<IUnitOfWork>(MockBehavior.Strict);

        _sut = new CurrentUserProvisioningService(_userRepo.Object, _uow.Object);
    }

    [Test]
    public async Task EnsureCurrentUserAsync_Should_ReturnUnauthorized_On_MissingExternalObjectId()
    {
        // Arrange
        var identity = new ExternalUserIdentity("", "tenant-id", "parent@example.com", "Avery", "Taylor");

        // Act
        var result = await _sut.EnsureCurrentUserAsync(identity, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.Unauthorized);
        result.Error!.Message.ShouldBe("External object id is required.");
        _userRepo.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task EnsureCurrentUserAsync_Should_CreateUserAndHousehold_On_NewExternalIdentity()
    {
        // Arrange
        var identity = new ExternalUserIdentity("external-object-id", "tenant-id", "parent@example.com", "Avery", "Taylor");
        Household? createdHousehold = null;

        _userRepo
            .Setup(r => r.GetByExternalIdentityAsync(identity.ExternalObjectId, identity.ExternalTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepo
            .Setup(r => r.GetByEmailAsync(identity.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepo
            .Setup(r => r.AddUser(It.Is<User>(user =>
                user.Email == identity.Email &&
                user.ExternalObjectId == identity.ExternalObjectId &&
                user.Status == UserAccountStatusEnum.Active)));
        _userRepo
            .Setup(r => r.GetMembershipAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((HouseholdId HouseholdId, CarerId? CarerId)?)null);
        _userRepo
            .Setup(r => r.AddHousehold(It.IsAny<Household>()))
            .Callback<Household>(household => createdHousehold = household);
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _userRepo
            .Setup(r => r.GetMembershipsAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => [
                (createdHousehold!.Id, createdHousehold.Carers.Single().Id)
            ]);

        // Act
        var result = await _sut.EnsureCurrentUserAsync(identity, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.FullName.ShouldBe("Avery Taylor");
        result.Value.HouseholdId.ShouldBe(createdHousehold!.Id);
        result.Value.CarerId.ShouldBe(createdHousehold.Carers.Single().Id);
        createdHousehold.Name.ShouldBe("Avery's household");
        _userRepo.Verify(r => r.GetByExternalIdentityAsync(identity.ExternalObjectId, identity.ExternalTenantId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepo.Verify(r => r.GetByEmailAsync(identity.Email, It.IsAny<CancellationToken>()), Times.Once);
        _userRepo.Verify(r => r.AddUser(It.IsAny<User>()), Times.Once);
        _userRepo.Verify(r => r.GetMembershipAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepo.Verify(r => r.AddHousehold(It.IsAny<Household>()), Times.Once);
        _userRepo.Verify(r => r.GetMembershipsAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _userRepo.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task EnsureCurrentUserAsync_Should_ReturnForbidden_On_DisabledUser()
    {
        // Arrange
        var identity = new ExternalUserIdentity("external-object-id", "tenant-id", "parent@example.com", "Avery", "Taylor");
        var user = User.CreateActiveParent(new UserId(Guid.NewGuid()), identity.Email, "Avery", "Taylor");
        user.Disable();

        _userRepo
            .Setup(r => r.GetByExternalIdentityAsync(identity.ExternalObjectId, identity.ExternalTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.EnsureCurrentUserAsync(identity, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.Forbidden);
        result.Error!.Message.ShouldBe("User account is disabled.");
        _userRepo.Verify(r => r.GetByExternalIdentityAsync(identity.ExternalObjectId, identity.ExternalTenantId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepo.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ResolveCurrentUserAsync_Should_ReturnResult_On_ExistingUserWithMembership()
    {
        // Arrange
        var identity = new ExternalUserIdentity("external-object-id", "tenant-id", "parent@example.com", "Avery", "Taylor");
        var user = User.CreateActiveParent(identity.Email, "Avery", "Taylor", identity.ExternalObjectId, identity.ExternalTenantId);
        var householdId = new HouseholdId(Guid.NewGuid());
        var carerId = new CarerId(Guid.NewGuid());

        _userRepo
            .Setup(r => r.GetByExternalIdentityAsync(identity.ExternalObjectId, identity.ExternalTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userRepo
            .Setup(r => r.GetMembershipsAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([(householdId, carerId)]);

        // Act
        var result = await _sut.ResolveCurrentUserAsync(identity, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.UserId.ShouldBe(user.Id);
        result.Value.HouseholdId.ShouldBe(householdId);
        result.Value.CarerId.ShouldBe(carerId);
        _userRepo.Verify(r => r.GetByExternalIdentityAsync(identity.ExternalObjectId, identity.ExternalTenantId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepo.Verify(r => r.GetMembershipsAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        _userRepo.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ResolveCurrentUserAsync_Should_ReturnUnauthorized_On_UserNotFound()
    {
        // Arrange
        var identity = new ExternalUserIdentity("external-object-id", "tenant-id", "parent@example.com", "Avery", "Taylor");

        _userRepo
            .Setup(r => r.GetByExternalIdentityAsync(identity.ExternalObjectId, identity.ExternalTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.ResolveCurrentUserAsync(identity, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.Unauthorized);
        result.Error!.Message.ShouldBe("User was not found.");
        _userRepo.Verify(r => r.GetByExternalIdentityAsync(identity.ExternalObjectId, identity.ExternalTenantId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepo.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }
}
