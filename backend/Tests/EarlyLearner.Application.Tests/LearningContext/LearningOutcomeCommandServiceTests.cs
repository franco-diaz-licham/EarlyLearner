using EarlyLearner.Application.Ports;
using EarlyLearner.Application.UseCases.LearningContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Domain.LearningContext.Entities;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using Moq;
using Shouldly;

namespace EarlyLearner.Application.Tests.LearningContext;

[TestFixture]
public sealed class LearningOutcomeCommandServiceTests
{
    private Mock<ILearningOutcomeCommandRepository> _learningOutcomeRepo = default!;
    private Mock<IUnitOfWork> _uow = default!;
    private Mock<ICurrentUser> _currentUser = default!;
    private LearningOutcomeCommandService _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _learningOutcomeRepo = new Mock<ILearningOutcomeCommandRepository>(MockBehavior.Strict);
        _uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _currentUser = new Mock<ICurrentUser>(MockBehavior.Strict);

        _sut = new LearningOutcomeCommandService(_learningOutcomeRepo.Object, _uow.Object, _currentUser.Object);
    }

    [Test]
    public async Task CreateAsync_Should_ReturnCreatedResult_On_ValidCommand()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var command = new CreateLearningOutcomeCommand("language-listening", "Listens and responds", "Listens to short instructions.", "Language", 10);
        LearningOutcome? addedOutcome = null;

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _learningOutcomeRepo
            .Setup(repo => repo.Add(It.IsAny<LearningOutcome>()))
            .Callback<LearningOutcome>(outcome => addedOutcome = outcome);
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _learningOutcomeRepo
            .Setup(repo => repo.GetResponseAsync(householdId, It.IsAny<LearningOutcomeId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => CreateResponse(addedOutcome!));

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Created);
        result.Value.Name.ShouldBe(command.Name);
        result.Value.Status.ShouldBe(LearningOutcomeStatusEnum.Active);
        addedOutcome.ShouldNotBeNull();
        addedOutcome.HouseholdId.ShouldBe(householdId);
        addedOutcome.Code.ShouldBe(command.Code);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _learningOutcomeRepo.Verify(repo => repo.Add(It.IsAny<LearningOutcome>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _learningOutcomeRepo.Verify(repo => repo.GetResponseAsync(householdId, addedOutcome.Id, It.IsAny<CancellationToken>()), Times.Once);
        _learningOutcomeRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnNotFound_On_MissingLearningOutcome()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var command = new UpdateLearningOutcomeCommand(new LearningOutcomeId(Guid.NewGuid()), "Name", "Description", "Language", 20);

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _learningOutcomeRepo
            .Setup(repo => repo.GetAsync(householdId, command.LearningOutcomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LearningOutcome?)null);

        // Act
        var result = await _sut.UpdateAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.NotFound);
        result.Error!.Message.ShouldBe("Learning outcome was not found.");
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _learningOutcomeRepo.Verify(repo => repo.GetAsync(householdId, command.LearningOutcomeId, It.IsAny<CancellationToken>()), Times.Once);
        _learningOutcomeRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateStatusAsync_Should_ReturnUpdatedResult_On_ValidStatus()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var outcome = CreateLearningOutcome(householdId);
        var command = new UpdateLearningOutcomeStatusCommand(outcome.Id, LearningOutcomeStatusEnum.Inactive);
        var response = CreateResponse(outcome) with { Status = LearningOutcomeStatusEnum.Inactive };

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _learningOutcomeRepo
            .Setup(repo => repo.GetAsync(householdId, command.LearningOutcomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _learningOutcomeRepo
            .Setup(repo => repo.GetResponseAsync(householdId, outcome.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _sut.UpdateStatusAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Updated);
        result.Value.ShouldBe(response);
        outcome.Status.ShouldBe(LearningOutcomeStatusEnum.Inactive);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _learningOutcomeRepo.Verify(repo => repo.GetAsync(householdId, command.LearningOutcomeId, It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _learningOutcomeRepo.Verify(repo => repo.GetResponseAsync(householdId, outcome.Id, It.IsAny<CancellationToken>()), Times.Once);
        _learningOutcomeRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DeleteAsync_Should_ReturnConflict_On_UsedLearningOutcome()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var outcome = CreateLearningOutcome(householdId);

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _learningOutcomeRepo
            .Setup(repo => repo.GetAsync(householdId, outcome.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);
        _learningOutcomeRepo
            .Setup(repo => repo.IsUsedByLearningMomentAsync(householdId, outcome.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.DeleteAsync(outcome.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.Conflict);
        result.Error!.Message.ShouldBe("Learning outcome is already used by learning moments. Archive it instead of deleting it.");
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _learningOutcomeRepo.Verify(repo => repo.GetAsync(householdId, outcome.Id, It.IsAny<CancellationToken>()), Times.Once);
        _learningOutcomeRepo.Verify(repo => repo.IsUsedByLearningMomentAsync(householdId, outcome.Id, It.IsAny<CancellationToken>()), Times.Once);
        _learningOutcomeRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    private static LearningOutcome CreateLearningOutcome(HouseholdId householdId)
    {
        return LearningOutcome.Create(householdId, "language-listening", "Listens and responds", "Listens to short instructions.", "Language", 10);
    }

    private static LearningOutcomeResponse CreateResponse(LearningOutcome outcome)
    {
        return new LearningOutcomeResponse(outcome.Id.Value, outcome.Code, outcome.Name, outcome.Description, outcome.Category, outcome.SortOrder, outcome.Status);
    }
}
