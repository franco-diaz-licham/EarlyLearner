using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;

namespace EarlyLearner.Shared.Tests.Seeders;

public sealed record HouseholdSeed(User User, Household Household);

public static class HouseholdSeeder
{
    public static HouseholdSeed CreateHousehold(
        string householdName = "Taylor Household",
        string email = "parent@example.com",
        string firstName = "Avery",
        string lastName = "Taylor")
    {
        var user = User.CreateActiveParent(new UserId(Guid.NewGuid()), email, firstName, lastName);
        var household = Household.Create(householdName, user.Id);
        return new HouseholdSeed(user, household);
    }

    public static Child AddChild(
        Household household,
        string firstName = "Mia",
        string lastName = "Taylor",
        DateOnly? dateOfBirth = null,
        StoredFileId? avatarStoredFileId = null)
    {
        return household.AddChild(firstName, lastName, dateOfBirth ?? new DateOnly(2021, 3, 14), avatarStoredFileId);
    }

    public static async Task SeedAsync(DatabaseContext db, params HouseholdSeed[] seeds)
    {
        db.Users.AddRange(seeds.Select(seed => seed.User));
        db.Households.AddRange(seeds.Select(seed => seed.Household));
        await db.SaveChangesAsync();
    }

    public static async Task SeedUsersAsync(DatabaseContext db, params User[] users)
    {
        db.Users.AddRange(users);
        await db.SaveChangesAsync();
    }
}
