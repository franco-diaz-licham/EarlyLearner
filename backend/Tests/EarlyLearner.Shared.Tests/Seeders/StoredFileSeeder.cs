using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;

namespace EarlyLearner.Shared.Tests.Seeders;

public static class StoredFileSeeder
{
    public static StoredFile CreateStoredFile(
        Household household,
        string fileName = "avatar.png",
        DateTimeOffset? uploadedAt = null,
        StoredFileMediaTypeEnum mediaType = StoredFileMediaTypeEnum.Photo)
    {
        return StoredFile.Create(
            household.Id,
            $"uploads/{fileName}",
            fileName,
            "image/png",
            1024,
            mediaType,
            uploadedAt ?? DateTimeOffset.UtcNow);
    }

    public static StoredFile CreateStoredFile(
        HouseholdId householdId,
        string fileName = "avatar.png",
        DateTimeOffset? uploadedAt = null,
        StoredFileMediaTypeEnum mediaType = StoredFileMediaTypeEnum.Photo)
    {
        return StoredFile.Create(
            householdId,
            $"uploads/{fileName}",
            fileName,
            "image/png",
            1024,
            mediaType,
            uploadedAt ?? DateTimeOffset.UtcNow);
    }

    public static async Task SeedAsync(DatabaseContext db, params StoredFile[] storedFiles)
    {
        db.StoredFiles.AddRange(storedFiles);
        await db.SaveChangesAsync();
    }
}
