using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using Moq;
using Shouldly;

namespace EarlyLearner.Application.Tests.IdentityContext;

[TestFixture]
public class UserQueryServiceTests
{
    private Mock<IUserQueryRepository> _userQueryRepo = default!;
    private Mock<ICurrentUser> _currentUser = default!;
    private UserQueryService _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _userQueryRepo = new Mock<IUserQueryRepository>(MockBehavior.Strict);
        _currentUser = new Mock<ICurrentUser>(MockBehavior.Strict);

        _sut = new UserQueryService(_userQueryRepo.Object, _currentUser.Object);
    }

    [Test]
    public async Task GetCurrentUserAsync_Should_ReturnUnauthorized_On_UnauthenticatedCurrentUser()
    {
        // Arrange
        _currentUser
            .SetupGet(user => user.IsAuthenticated)
            .Returns(false);

        // Act
        var result = await _sut.GetCurrentUserAsync(new UserId(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.Unauthorized);
        result.Error!.Message.ShouldBe("Current user is not authenticated.");
        _currentUser.VerifyGet(user => user.IsAuthenticated, Times.Once);
        _currentUser.VerifyNoOtherCalls();
        _userQueryRepo.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetCurrentUserAsync_Should_ReturnForbidden_On_RequestedUserMismatch()
    {
        // Arrange
        var currentUserId = new UserId(Guid.NewGuid());
        var requestedUserId = new UserId(Guid.NewGuid());

        _currentUser
            .SetupGet(user => user.IsAuthenticated)
            .Returns(true);
        _currentUser
            .SetupGet(user => user.UserId)
            .Returns(currentUserId);

        // Act
        var result = await _sut.GetCurrentUserAsync(requestedUserId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.Forbidden);
        result.Error!.Message.ShouldBe("Current user does not match the requested user.");
        _currentUser.VerifyGet(user => user.IsAuthenticated, Times.Once);
        _currentUser.VerifyGet(user => user.UserId, Times.Once);
        _currentUser.VerifyNoOtherCalls();
        _userQueryRepo.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetCurrentUserAsync_Should_ReturnResult_On_AuthenticatedMatchingUser()
    {
        // Arrange
        var user = User.CreateActiveParent(new UserId(Guid.NewGuid()), "parent@example.com", "Avery", "Taylor");
        var householdId = new HouseholdId(Guid.NewGuid());
        var carerId = new CarerId(Guid.NewGuid());
        var accessibleHouseholdIds = new[] { householdId };

        _currentUser
            .SetupGet(current => current.IsAuthenticated)
            .Returns(true);
        _currentUser
            .SetupGet(current => current.UserId)
            .Returns(user.Id);
        _userQueryRepo
            .Setup(repo => repo.GetAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _currentUser
            .SetupGet(current => current.HouseholdId)
            .Returns(householdId);
        _currentUser
            .SetupGet(current => current.AccessibleHouseholdIds)
            .Returns(accessibleHouseholdIds);
        _currentUser
            .SetupGet(current => current.CarerId)
            .Returns(carerId);

        // Act
        var result = await _sut.GetCurrentUserAsync(user.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.FullName.ShouldBe("Avery Taylor");
        result.Value.UserId.ShouldBe(user.Id);
        result.Value.HouseholdId.ShouldBe(householdId);
        result.Value.AccessibleHouseholdIds.ShouldBe(accessibleHouseholdIds);
        result.Value.CarerId.ShouldBe(carerId);
        _currentUser.VerifyGet(current => current.IsAuthenticated, Times.Once);
        _currentUser.VerifyGet(current => current.UserId, Times.Once);
        _currentUser.VerifyGet(current => current.HouseholdId, Times.Once);
        _currentUser.VerifyGet(current => current.AccessibleHouseholdIds, Times.Once);
        _currentUser.VerifyGet(current => current.CarerId, Times.Once);
        _userQueryRepo.Verify(repo => repo.GetAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyNoOtherCalls();
        _userQueryRepo.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetUserByEmailAsync_Should_ReturnInvalid_On_MissingEmail()
    {
        // Arrange
        const string email = "";

        // Act
        var result = await _sut.GetUserByEmailAsync(email, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.Invalid);
        result.Error!.Message.ShouldBe("Email is required.");
        _currentUser.VerifyNoOtherCalls();
        _userQueryRepo.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetUserByObjectIdAsync_Should_ReturnResult_On_UserWithMembership()
    {
        // Arrange
        const string objectId = "external-object-id";
        var user = User.CreateActiveParent("parent@example.com", "Avery", "Taylor", objectId, externalTenantId: null);
        var householdId = new HouseholdId(Guid.NewGuid());
        var carerId = new CarerId(Guid.NewGuid());

        _userQueryRepo
            .Setup(repo => repo.GetByExternalObjectIdAsync(objectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userQueryRepo
            .Setup(repo => repo.GetMembershipsAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([(householdId, carerId)]);

        // Act
        var result = await _sut.GetUserByObjectIdAsync(objectId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.UserId.ShouldBe(user.Id);
        result.Value.HouseholdId.ShouldBe(householdId);
        result.Value.CarerId.ShouldBe(carerId);
        _userQueryRepo.Verify(repo => repo.GetByExternalObjectIdAsync(objectId, It.IsAny<CancellationToken>()), Times.Once);
        _userQueryRepo.Verify(repo => repo.GetMembershipsAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyNoOtherCalls();
        _userQueryRepo.VerifyNoOtherCalls();
    }
}
