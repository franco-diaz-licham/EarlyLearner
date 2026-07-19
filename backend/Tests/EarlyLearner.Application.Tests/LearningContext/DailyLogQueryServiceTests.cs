using EarlyLearner.Application.Ports;
using EarlyLearner.Application.UseCases.LearningContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using Moq;
using Shouldly;

namespace EarlyLearner.Application.Tests.LearningContext;

[TestFixture]
public sealed class DailyLogQueryServiceTests
{
    private Mock<IDailyLogQueryRepository> _dailyLogRepo = default!;
    private Mock<ICurrentUser> _currentUser = default!;
    private DailyLogQueryService _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _dailyLogRepo = new Mock<IDailyLogQueryRepository>(MockBehavior.Strict);
        _currentUser = new Mock<ICurrentUser>(MockBehavior.Strict);

        _sut = new DailyLogQueryService(_dailyLogRepo.Object, _currentUser.Object);
    }

    [Test]
    public async Task ListAsync_Should_ReturnDailyLogs_ForCurrentHousehold()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var logs = new List<DailyLogResponse> {
            CreateDailyLogResponse(householdId, new ChildId(Guid.NewGuid()))
        };

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _dailyLogRepo
            .Setup(repo => repo.ListAsync(householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        // Act
        var result = await _sut.ListAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.ShouldBe(logs);
        result.TotalCount.ShouldBe(logs.Count);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _dailyLogRepo.Verify(repo => repo.ListAsync(householdId, It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ListLearningMomentsAsync_Should_ReturnFeed_ForCurrentHousehold()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var childId = new ChildId(Guid.NewGuid());
        var query = new ListLearningMomentsQuery(PageNumber: 1, PageSize: 10, childId, SearchTerm: "paint");
        var items = new List<LearningMomentFeedResponse> {
            new LearningMomentFeedResponse(Guid.NewGuid(), Guid.NewGuid(), householdId.Value, childId.Value, new DateOnly(2026, 7, 18), LearningMomentKindEnum.Activity, "Paint", "Mixed colours.", [])
        };

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _dailyLogRepo
            .Setup(repo => repo.ListLearningMomentsAsync(householdId, query, It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, 12));

        // Act
        var result = await _sut.ListLearningMomentsAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.ShouldBe(items);
        result.TotalCount.ShouldBe(12);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _dailyLogRepo.Verify(repo => repo.ListLearningMomentsAsync(householdId, query, It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetAsync_Should_ReturnNotFound_On_MissingDailyLog()
    {
        // Arrange
        var dailyLogId = new DailyLogId(Guid.NewGuid());

        _dailyLogRepo
            .Setup(repo => repo.GetResponseAsync(dailyLogId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DailyLogResponse?)null);

        // Act
        var result = await _sut.GetAsync(dailyLogId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.NotFound);
        result.Error!.Message.ShouldBe("Daily log was not found.");
        _dailyLogRepo.Verify(repo => repo.GetResponseAsync(dailyLogId, It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
    }

    private static DailyLogResponse CreateDailyLogResponse(HouseholdId householdId, ChildId childId)
    {
        return new DailyLogResponse(Guid.NewGuid(), householdId.Value, childId.Value, new DateOnly(2026, 7, 18), LearningMomentCount: 1, LearningMoments: []);
    }
}

