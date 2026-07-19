using EarlyLearner.Application.UseCases.LearningContext;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Infrastructure.Persistence.Repositories;
using EarlyLearner.Shared.Tests;
using EarlyLearner.Shared.Tests.Seeders;
using Shouldly;

namespace EarlyLearner.Infrastructure.Tests.Persistence.Repositories;

[TestFixture]
public sealed class DailyLogRepositoryTests : BaseDatabaseSetup
{
    [Test]
    public async Task ChildExistsAsync_Should_ReturnTrue_WhenChildBelongsToHousehold()
    {
        // Arrange
        var seed = HouseholdSeeder.CreateHousehold();
        var child = HouseholdSeeder.AddChild(seed.Household);
        await HouseholdSeeder.SeedAsync(Db, seed);
        var sut = new DailyLogRepository(Db);

        // Act
        var result = await sut.ChildExistsAsync(seed.Household.Id, child.Id, CancellationToken.None);

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public async Task ListLearningMomentsAsync_Should_FilterByChildAndSearchTerm()
    {
        // Arrange
        var seed = HouseholdSeeder.CreateHousehold();
        var mia = HouseholdSeeder.AddChild(seed.Household, firstName: "Mia");
        var noah = HouseholdSeeder.AddChild(seed.Household, firstName: "Noah", dateOfBirth: new DateOnly(2020, 9, 8));
        var outcome = LearningSeeder.CreateOutcome();
        var miaLog = LearningSeeder.CreateDailyLog(seed.Household, mia, title: "Paint mixing", notes: "Mixed colours with brushes.", outcomes: [outcome]);
        var noahLog = LearningSeeder.CreateDailyLog(seed.Household, noah, kind: LearningMomentKindEnum.Reading, title: "Picture story", notes: "Read quietly.", outcomes: [outcome]);
        await HouseholdSeeder.SeedAsync(Db, seed);
        await LearningSeeder.SeedOutcomesAsync(Db, outcome);
        await LearningSeeder.SeedDailyLogsAsync(Db, miaLog, noahLog);
        var query = new ListLearningMomentsQuery(PageNumber: 1, PageSize: 10, mia.Id, SearchTerm: "paint");
        var sut = new DailyLogRepository(Db);

        // Act
        var result = await sut.ListLearningMomentsAsync(seed.Household.Id, query, CancellationToken.None);

        // Assert
        result.TotalCount.ShouldBe(1);
        result.Items.Single().ChildId.ShouldBe(mia.Id.Value);
        result.Items.Single().Title.ShouldBe("Paint mixing");
    }

    [Test]
    public async Task GetResponseAsync_Should_ProjectLearningMomentsWithOutcomeIds()
    {
        // Arrange
        var seed = HouseholdSeeder.CreateHousehold();
        var child = HouseholdSeeder.AddChild(seed.Household);
        var outcome = LearningSeeder.CreateOutcome();
        var dailyLog = LearningSeeder.CreateDailyLog(seed.Household, child, outcomes: [outcome]);
        var moment = dailyLog.LearningMoments.Single();
        await HouseholdSeeder.SeedAsync(Db, seed);
        await LearningSeeder.SeedOutcomesAsync(Db, outcome);
        await LearningSeeder.SeedDailyLogsAsync(Db, dailyLog);
        var sut = new DailyLogRepository(Db);

        // Act
        var result = await sut.GetResponseAsync(dailyLog.Id, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.DailyLogId.ShouldBe(dailyLog.Id.Value);
        result.LearningMomentCount.ShouldBe(1);
        result.LearningMoments.Single().LearningMomentId.ShouldBe(moment.Id.Value);
        result.LearningMoments.Single().LearningOutcomeIds.ShouldBe([outcome.Id.Value]);
    }
}
