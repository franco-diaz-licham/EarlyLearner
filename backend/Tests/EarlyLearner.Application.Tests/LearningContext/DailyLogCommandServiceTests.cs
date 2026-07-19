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
public sealed class DailyLogCommandServiceTests
{
    private Mock<IDailyLogCommandRepository> _dailyLogRepo = default!;
    private Mock<IUnitOfWork> _uow = default!;
    private Mock<ICurrentUser> _currentUser = default!;
    private DailyLogCommandService _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _dailyLogRepo = new Mock<IDailyLogCommandRepository>(MockBehavior.Strict);
        _uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _currentUser = new Mock<ICurrentUser>(MockBehavior.Strict);

        _sut = new DailyLogCommandService(_dailyLogRepo.Object, _uow.Object, _currentUser.Object);
    }

    [Test]
    public async Task CreateAsync_Should_ReturnCreatedDailyLog_On_NewChildLog()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var childId = new ChildId(Guid.NewGuid());
        var outcome = CreateLearningOutcome("language-listening", "Listens and responds");
        var command = new CreateDailyLogCommand(childId, new DateOnly(2026, 7, 18), LearningMomentKindEnum.Activity, "Paint mixing", "Mixed colours.", [outcome.Id]);
        DailyLog? addedLog = null;

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _dailyLogRepo
            .Setup(repo => repo.ChildExistsAsync(householdId, childId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _dailyLogRepo
            .Setup(repo => repo.GetLearningOutcomesAsync(command.LearningOutcomeIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync([outcome]);
        _dailyLogRepo
            .Setup(repo => repo.GetByChildAndDateAsync(householdId, childId, command.LogDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DailyLog?)null);
        _dailyLogRepo
            .Setup(repo => repo.Add(It.IsAny<DailyLog>()))
            .Callback<DailyLog>(log => addedLog = log);
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _dailyLogRepo
            .Setup(repo => repo.GetResponseAsync(It.IsAny<DailyLogId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new DailyLogResponse(addedLog!.Id.Value, householdId.Value, childId.Value, command.LogDate, LearningMomentCount: 1, LearningMoments: [
                new LearningMomentResponse(addedLog.LearningMoments.Single().Id.Value, command.Kind, command.Title, command.Notes, [outcome.Id.Value])
            ]));

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Created);
        result.Value.ChildId.ShouldBe(childId.Value);
        result.Value.LearningMomentCount.ShouldBe(1);
        addedLog.ShouldNotBeNull();
        addedLog.LearningMoments.Single().Title.ShouldBe(command.Title);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Exactly(3));
        _dailyLogRepo.Verify(repo => repo.ChildExistsAsync(householdId, childId, It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.Verify(repo => repo.GetLearningOutcomesAsync(command.LearningOutcomeIds, It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.Verify(repo => repo.GetByChildAndDateAsync(householdId, childId, command.LogDate, It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.Verify(repo => repo.Add(It.IsAny<DailyLog>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.Verify(repo => repo.GetResponseAsync(addedLog.Id, It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task CreateAsync_Should_ReturnNotFound_WhenChildDoesNotBelongToHousehold()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var childId = new ChildId(Guid.NewGuid());
        var command = new CreateDailyLogCommand(childId, new DateOnly(2026, 7, 18), LearningMomentKindEnum.Activity, "Paint mixing", "Mixed colours.", []);

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _dailyLogRepo
            .Setup(repo => repo.ChildExistsAsync(householdId, childId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.NotFound);
        result.Error!.Message.ShouldBe("Child was not found in this household.");
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _dailyLogRepo.Verify(repo => repo.ChildExistsAsync(householdId, childId, It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateLearningMomentAsync_Should_ReturnUpdatedDailyLog_On_ValidCommand()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var childId = new ChildId(Guid.NewGuid());
        var dailyLog = DailyLog.Create(householdId, childId, new DateOnly(2026, 7, 18));
        var originalOutcome = CreateLearningOutcome("language-listening", "Listens and responds");
        var updatedOutcome = CreateLearningOutcome("social-turn-taking", "Takes turns with others");
        var moment = dailyLog.RecordLearningMoment(LearningMomentKindEnum.Activity, "Paint mixing", "Mixed colours.", [originalOutcome]);
        var command = new UpdateLearningMomentCommand(dailyLog.Id, moment.Id, LearningMomentKindEnum.Reading, "Updated story", "Read a picture book.", [updatedOutcome.Id]);
        var response = new DailyLogResponse(
            dailyLog.Id.Value,
            householdId.Value,
            childId.Value,
            dailyLog.LogDate,
            LearningMomentCount: 1,
            LearningMoments: [new LearningMomentResponse(moment.Id.Value, LearningMomentKindEnum.Reading, command.Title, command.Notes, [updatedOutcome.Id.Value])]);

        _dailyLogRepo
            .Setup(repo => repo.GetAsync(command.DailyLogId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dailyLog);
        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _dailyLogRepo
            .Setup(repo => repo.GetLearningOutcomesAsync(command.LearningOutcomeIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync([updatedOutcome]);
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _dailyLogRepo
            .Setup(repo => repo.GetResponseAsync(dailyLog.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _sut.UpdateLearningMomentAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.ShouldBe(response);
        moment.Kind.ShouldBe(command.Kind);
        moment.Title.ShouldBe(command.Title);
        moment.Notes.ShouldBe(command.Notes);
        moment.LearningOutcomes.Single().ShouldBe(updatedOutcome);
        _dailyLogRepo.Verify(repo => repo.GetAsync(command.DailyLogId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _dailyLogRepo.Verify(repo => repo.GetLearningOutcomesAsync(command.LearningOutcomeIds, It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.Verify(repo => repo.GetResponseAsync(dailyLog.Id, It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateLearningMomentAsync_Should_ReturnNotFound_WhenDailyLogDoesNotBelongToHousehold()
    {
        // Arrange
        var dailyLog = DailyLog.Create(new HouseholdId(Guid.NewGuid()), new ChildId(Guid.NewGuid()), new DateOnly(2026, 7, 18));
        var command = new UpdateLearningMomentCommand(dailyLog.Id, new LearningMomentId(Guid.NewGuid()), LearningMomentKindEnum.Reading, "Story", "Notes", [new LearningOutcomeId(Guid.NewGuid())]);

        _dailyLogRepo
            .Setup(repo => repo.GetAsync(command.DailyLogId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dailyLog);
        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(new HouseholdId(Guid.NewGuid()));

        // Act
        var result = await _sut.UpdateLearningMomentAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.NotFound);
        result.Error!.Message.ShouldBe("Daily log was not found.");
        _dailyLogRepo.Verify(repo => repo.GetAsync(command.DailyLogId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _dailyLogRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DeleteLearningMomentAsync_Should_ReturnSuccess_On_ExistingMoment()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var dailyLog = DailyLog.Create(householdId, new ChildId(Guid.NewGuid()), new DateOnly(2026, 7, 18));
        var outcome = CreateLearningOutcome("language-listening", "Listens and responds");
        var moment = dailyLog.RecordLearningMoment(LearningMomentKindEnum.Activity, "Paint mixing", "Mixed colours.", [outcome]);

        _dailyLogRepo
            .Setup(repo => repo.GetAsync(dailyLog.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dailyLog);
        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _dailyLogRepo
            .Setup(repo => repo.RemoveLearningMoment(moment));
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.DeleteLearningMomentAsync(dailyLog.Id, moment.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        dailyLog.LearningMoments.ShouldBeEmpty();
        _dailyLogRepo.Verify(repo => repo.GetAsync(dailyLog.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _dailyLogRepo.Verify(repo => repo.RemoveLearningMoment(moment), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DeleteLearningMomentAsync_Should_ReturnNotFound_On_MissingDailyLog()
    {
        // Arrange
        var dailyLogId = new DailyLogId(Guid.NewGuid());
        var learningMomentId = new LearningMomentId(Guid.NewGuid());

        _dailyLogRepo
            .Setup(repo => repo.GetAsync(dailyLogId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DailyLog?)null);

        // Act
        var result = await _sut.DeleteLearningMomentAsync(dailyLogId, learningMomentId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.NotFound);
        result.Error!.Message.ShouldBe("Daily log was not found.");
        _dailyLogRepo.Verify(repo => repo.GetAsync(dailyLogId, It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    private static LearningOutcome CreateLearningOutcome(string code, string name)
    {
        return LearningOutcome.Create(code, name, "Description", "Language", 10);
    }
}

