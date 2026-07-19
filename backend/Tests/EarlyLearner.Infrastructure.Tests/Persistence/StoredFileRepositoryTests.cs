using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Infrastructure.Persistence.Repositories;
using EarlyLearner.Shared.Tests;
using EarlyLearner.Shared.Tests.Seeders;
using Shouldly;

namespace EarlyLearner.Infrastructure.Tests.Persistence.Repositories;

[TestFixture]
public sealed class StoredFileRepositoryTests : BaseDatabaseSetup
{
    [Test]
    public async Task ListAsync_Should_ReturnFilesForHouseholdOrderedByUploadedAtDescending()
    {
        // Arrange
        var seed = HouseholdSeeder.CreateHousehold();
        var otherSeed = HouseholdSeeder.CreateHousehold("Other Household", "other@example.com");
        var olderFile = StoredFileSeeder.CreateStoredFile(seed.Household, "older.png", new DateTimeOffset(2026, 7, 17, 10, 0, 0, TimeSpan.Zero));
        var newerFile = StoredFileSeeder.CreateStoredFile(seed.Household, "newer.png", new DateTimeOffset(2026, 7, 18, 10, 0, 0, TimeSpan.Zero));
        var otherFile = StoredFileSeeder.CreateStoredFile(otherSeed.Household, "other.png", new DateTimeOffset(2026, 7, 19, 10, 0, 0, TimeSpan.Zero));
        await HouseholdSeeder.SeedAsync(Db, seed, otherSeed);
        await StoredFileSeeder.SeedAsync(Db, olderFile, newerFile, otherFile);
        var sut = new StoredFileRepository(Db);

        // Act
        var result = await sut.ListAsync(seed.Household.Id, CancellationToken.None);

        // Assert
        result.Select(file => file.StoredFileId).ShouldBe([newerFile.Id.Value, olderFile.Id.Value]);
        result.ShouldAllBe(file => file.HouseholdId == seed.Household.Id.Value);
    }

    [Test]
    public async Task GetResponseAsync_Should_ReturnStoredFileProjection_On_ExistingFile()
    {
        // Arrange
        var seed = HouseholdSeeder.CreateHousehold();
        var file = StoredFileSeeder.CreateStoredFile(seed.Household);
        await HouseholdSeeder.SeedAsync(Db, seed);
        await StoredFileSeeder.SeedAsync(Db, file);
        var sut = new StoredFileRepository(Db);

        // Act
        var result = await sut.GetResponseAsync(file.Id, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.StoredFileId.ShouldBe(file.Id.Value);
        result.HouseholdId.ShouldBe(seed.Household.Id.Value);
        result.FileName.ShouldBe(file.FileName);
        result.MediaType.ShouldBe(StoredFileMediaTypeEnum.Photo);
    }
}
