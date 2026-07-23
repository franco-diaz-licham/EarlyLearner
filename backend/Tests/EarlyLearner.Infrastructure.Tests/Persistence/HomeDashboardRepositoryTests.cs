using EarlyLearner.Application.Ports;
using EarlyLearner.Application.UseCases.Dashboard;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Infrastructure.Persistence.Repositories;
using EarlyLearner.Shared.Tests;
using EarlyLearner.Shared.Tests.Seeders;
using EarlyLearner.Shared.Utilities;
using Moq;
using Shouldly;

namespace EarlyLearner.Infrastructure.Tests.Persistence;

[TestFixture]
public sealed class HomeDashboardRepositoryTests : BaseDatabaseSetup
{
    private Mock<ICurrentUser> _currentUser = default!;

    [SetUp]
    public void SetUp()
    {
        _currentUser = new Mock<ICurrentUser>(MockBehavior.Strict);
    }

    [Test]
    public async Task GetAsync_Should_ReturnDashboardForCurrentHousehold()
    {
        // Arrange
        var today = new DateOnly(2026, 7, 19);
        var seed = HouseholdSeeder.CreateHousehold();
        var otherSeed = HouseholdSeeder.CreateHousehold("Other Household", "other@example.com");
        var mia = HouseholdSeeder.AddChild(seed.Household, firstName: "Mia");
        var noah = HouseholdSeeder.AddChild(seed.Household, firstName: "Noah", dateOfBirth: new DateOnly(2020, 9, 8));
        var archivedChild = HouseholdSeeder.AddChild(seed.Household, firstName: "Zoe", dateOfBirth: new DateOnly(2019, 5, 1));
        var otherChild = HouseholdSeeder.AddChild(otherSeed.Household, firstName: "Other", lastName: "Child", dateOfBirth: new DateOnly(2020, 1, 1));
        seed.Household.ArchiveChild(archivedChild.Id);
        var touchedOutcome = LearningSeeder.CreateOutcome("language-listening", "Listens and responds", "Listens to instructions.", "Language", 10, seed.Household.Id);
        var untouchedOutcome = LearningSeeder.CreateOutcome("social-turn-taking", "Takes turns with others", "Practices waiting.", "Social", 20, seed.Household.Id);
        var inactiveOutcome = LearningSeeder.CreateOutcome("archived-outcome", "Archived outcome", "Old outcome.", "Archive", 30, seed.Household.Id);
        var otherOutcome = LearningSeeder.CreateOutcome("other-outcome", "Other outcome", "Not visible.", "Other", 40, otherSeed.Household.Id);
        inactiveOutcome.Deactivate();
        var todayLog = LearningSeeder.CreateDailyLog(seed.Household, mia, today, LearningMomentKindEnum.Activity, "Paint mixing", "Mixed colours.", touchedOutcome);
        var earlierLog = LearningSeeder.CreateDailyLog(seed.Household, noah, today.AddDays(-2), LearningMomentKindEnum.Reading, "Picture story", "Read quietly.", touchedOutcome);
        var otherLog = LearningSeeder.CreateDailyLog(otherSeed.Household, otherChild, today, LearningMomentKindEnum.Activity, "Other activity", "Not visible.", otherOutcome);
        await HouseholdSeeder.SeedAsync(Db, seed, otherSeed);
        await LearningSeeder.SeedOutcomesAsync(Db, touchedOutcome, untouchedOutcome, inactiveOutcome, otherOutcome);
        await LearningSeeder.SeedDailyLogsAsync(Db, todayLog, earlierLog, otherLog);
        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(seed.Household.Id);
        var sut = new HomeDashboardRepository(Db, _currentUser.Object);

        // Act
        var result = await sut.GetAsync(new GetHomeDashboardQuery(today), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.Children.Select(child => child.GivenName).ShouldBe(["Mia", "Noah"]);
        result.Value.Today.DailyLogCount.ShouldBe(1);
        result.Value.Today.LearningMomentCount.ShouldBe(1);
        result.Value.Today.ChildrenObservedCount.ShouldBe(1);
        result.Value.OutcomeCoverage.ActiveOutcomeCount.ShouldBe(2);
        result.Value.OutcomeCoverage.TouchedThisWeekCount.ShouldBe(1);
        result.Value.OutcomeCoverage.UntouchedActiveOutcomeCount.ShouldBe(1);
        result.Value.RecentMoments.ShouldAllBe(moment => moment.ChildId != otherChild.Id.Value);
        result.Value.RecentMoments.First().Title.ShouldBe("Paint mixing");
        result.Value.Metrics.Single(metric => metric.Label == "Active children").Value.ShouldBe(2);
        result.Value.Metrics.Single(metric => metric.Label == "Moments this week").Value.ShouldBe(2);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _currentUser.VerifyNoOtherCalls();
    }
}
