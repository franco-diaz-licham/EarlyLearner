using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Infrastructure.Persistence.Repositories;
using EarlyLearner.Shared.Tests;
using EarlyLearner.Shared.Tests.Seeders;
using Shouldly;

namespace EarlyLearner.Infrastructure.Tests.Persistence.Repositories;

[TestFixture]
public sealed class LearningOutcomeRepositoryTests : BaseDatabaseSetup
{
    [Test]
    public async Task ListAsync_Should_ReturnOutcomesOrderedBySortOrder()
    {
        // Arrange
        var laterOutcome = LearningSeeder.CreateOutcome("motor-mark-making", "Uses early mark making", "Uses crayons.", "Motor", 20);
        var earlierOutcome = LearningSeeder.CreateOutcome("language-listening", "Listens and responds", "Listens to instructions.", "Language", 10);
        await LearningSeeder.SeedOutcomesAsync(Db, laterOutcome, earlierOutcome);
        var sut = new LearningOutcomeRepository(Db);

        // Act
        var result = await sut.ListAsync(CancellationToken.None);

        // Assert
        result.Select(outcome => outcome.LearningOutcomeId).ShouldBe([earlierOutcome.Id.Value, laterOutcome.Id.Value]);
        result.Select(outcome => outcome.Name).ShouldBe([earlierOutcome.Name, laterOutcome.Name]);
    }

    [Test]
    public async Task IsUsedByLearningMomentAsync_Should_ReturnTrue_WhenOutcomeIsLinkedToMoment()
    {
        // Arrange
        var seed = HouseholdSeeder.CreateHousehold();
        var child = HouseholdSeeder.AddChild(seed.Household);
        var outcome = LearningSeeder.CreateOutcome();
        var dailyLog = LearningSeeder.CreateDailyLog(seed.Household, child, kind: LearningMomentKindEnum.Activity, outcomes: [outcome]);
        await HouseholdSeeder.SeedAsync(Db, seed);
        await LearningSeeder.SeedOutcomesAsync(Db, outcome);
        await LearningSeeder.SeedDailyLogsAsync(Db, dailyLog);
        var sut = new LearningOutcomeRepository(Db);

        // Act
        var result = await sut.IsUsedByLearningMomentAsync(outcome.Id, CancellationToken.None);

        // Assert
        result.ShouldBeTrue();
    }
}
