using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence.Repositories;
using EarlyLearner.Shared.Tests;
using EarlyLearner.Shared.Tests.Seeders;
using Shouldly;

namespace EarlyLearner.Infrastructure.Tests.Persistence.Repositories;

[TestFixture]
public sealed class HouseholdRepositoryTests : BaseDatabaseSetup
{
    [Test]
    public async Task ListAsync_Should_ReturnAccessibleHouseholdsWithActiveChildrenOnly()
    {
        // Arrange
        var firstSeed = HouseholdSeeder.CreateHousehold("B Household", "first@example.com");
        var secondSeed = HouseholdSeeder.CreateHousehold("A Household", "second@example.com");
        var hiddenSeed = HouseholdSeeder.CreateHousehold("Hidden Household", "hidden@example.com");
        var activeChild = HouseholdSeeder.AddChild(firstSeed.Household, avatarStoredFileId: new StoredFileId(Guid.NewGuid()));
        var archivedChild = HouseholdSeeder.AddChild(firstSeed.Household, firstName: "Noah", dateOfBirth: new DateOnly(2020, 9, 8));
        firstSeed.Household.ArchiveChild(archivedChild.Id);
        await HouseholdSeeder.SeedAsync(Db, firstSeed, secondSeed, hiddenSeed);
        var sut = new HouseholdRepository(Db);

        // Act
        var result = await sut.ListAsync([firstSeed.Household.Id, secondSeed.Household.Id], CancellationToken.None);

        // Assert
        result.Select(household => household.Id).ShouldBe([secondSeed.Household.Id.Value, firstSeed.Household.Id.Value]);
        result.ShouldNotContain(household => household.Id == hiddenSeed.Household.Id.Value);
        var firstResponse = result.Single(household => household.Id == firstSeed.Household.Id.Value);
        firstResponse.Children.Single().Id.ShouldBe(activeChild.Id.Value);
        firstResponse.Children.Single().AvatarStoredFileId.ShouldBe(activeChild.AvatarStoredFileId!.Value.Value);
    }

    [Test]
    public async Task ListAsync_Should_ReturnEmptyList_On_NoAccessibleHouseholds()
    {
        // Arrange
        var seed = HouseholdSeeder.CreateHousehold();
        await HouseholdSeeder.SeedAsync(Db, seed);
        var sut = new HouseholdRepository(Db);

        // Act
        var result = await sut.ListAsync([], CancellationToken.None);

        // Assert
        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GetUserByEmailAsync_Should_MatchNormalizedEmail()
    {
        // Arrange
        var user = User.CreateActiveParent(new UserId(Guid.NewGuid()), "parent@example.com", "Avery", "Taylor");
        await HouseholdSeeder.SeedUsersAsync(Db, user);
        var sut = new HouseholdRepository(Db);

        // Act
        var result = await sut.GetUserByEmailAsync(" Parent@Example.Com ", CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(user.Id);
    }
}
