using EarlyLearner.Application.UseCases.IdentityContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using Moq;
using Shouldly;

namespace EarlyLearner.Application.Tests.IdentityContext;

[TestFixture]
public class HouseholdCommandServiceTests
{
    private Mock<IHouseholdCommandRepository> _householdRepo = default!;
    private Mock<IUnitOfWork> _uow = default!;
    private Mock<ICurrentUser> _currentUser = default!;
    private HouseholdCommandService _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _householdRepo = new Mock<IHouseholdCommandRepository>(MockBehavior.Strict);
        _uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _currentUser = new Mock<ICurrentUser>(MockBehavior.Strict);

        _sut = new HouseholdCommandService(_householdRepo.Object, _uow.Object, _currentUser.Object);
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnUpdatedResult_On_ValidCommand()
    {
        // Arrange
        var household = Household.Create("Taylor Household", new UserId(Guid.NewGuid()));
        var response = new HouseholdResponse(household.Id.Value, "New household name", Carers: [], Children: [], Invitations: []);
        var command = new UpdateHouseholdCommand("New household name");

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(household.Id);
        _householdRepo
            .Setup(repo => repo.GetAsync(household.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(household);
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _householdRepo
            .Setup(repo => repo.GetResponseAsync(household.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _sut.UpdateAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Updated);
        result.Value.ShouldBe(response);
        household.Name.ShouldBe(command.Name);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _householdRepo.Verify(repo => repo.GetAsync(household.Id, It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _householdRepo.Verify(repo => repo.GetResponseAsync(household.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyNoOtherCalls();
        _householdRepo.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddCarerAsync_Should_CreateInvitation_On_NewCarerEmail()
    {
        // Arrange
        var household = Household.Create("Taylor Household", new UserId(Guid.NewGuid()));
        var response = new HouseholdResponse(household.Id.Value, household.Name, Carers: [], Children: [], Invitations: []);
        var command = new AddHouseholdCarerCommand("caregiver@example.com", HouseholdRoleEnum.Caregiver);

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(household.Id);
        _householdRepo
            .Setup(repo => repo.GetAsync(household.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(household);
        _householdRepo
            .Setup(repo => repo.GetUserByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _currentUser
            .SetupGet(user => user.UserId)
            .Returns(new UserId(Guid.NewGuid()));
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _householdRepo
            .Setup(repo => repo.GetResponseAsync(household.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _sut.AddCarerAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Updated);
        household.Invitations.Single().Email.ShouldBe(command.Email);
        household.Invitations.Single().Role.ShouldBe(command.Role);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _currentUser.VerifyGet(user => user.UserId, Times.Once);
        _householdRepo.Verify(repo => repo.GetAsync(household.Id, It.IsAny<CancellationToken>()), Times.Once);
        _householdRepo.Verify(repo => repo.GetUserByEmailAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _householdRepo.Verify(repo => repo.GetResponseAsync(household.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyNoOtherCalls();
        _householdRepo.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddChildAsync_Should_ReturnInvalid_On_SaveFailure()
    {
        // Arrange
        var household = Household.Create("Taylor Household", new UserId(Guid.NewGuid()));
        var command = new AddHouseholdChildCommand("Mia", "Taylor", new DateOnly(2021, 3, 14), AvatarStoredFileId: null);

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(household.Id);
        _householdRepo
            .Setup(repo => repo.GetAsync(household.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(household);
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _sut.AddChildAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.Invalid);
        result.Error!.Message.ShouldBe("Child could not be added.");
        household.Children.Count.ShouldBe(1);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _householdRepo.Verify(repo => repo.GetAsync(household.Id, It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyNoOtherCalls();
        _householdRepo.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task RemoveCarerAsync_Should_ReturnNotFound_On_MissingHousehold()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var command = new RemoveHouseholdCarerCommand(new CarerId(Guid.NewGuid()));

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _householdRepo
            .Setup(repo => repo.GetAsync(householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Household?)null);

        // Act
        var result = await _sut.RemoveCarerAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.NotFound);
        result.Error!.Message.ShouldBe("Household was not found.");
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _householdRepo.Verify(repo => repo.GetAsync(householdId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyNoOtherCalls();
        _householdRepo.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }
}
