using EarlyLearner.Application.Features.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.CoreContext;

public sealed class EfStoredFileCommandRepository(DatabaseContext db) : IStoredFileCommandRepository
{
    public Task<bool> HouseholdExistsAsync(Guid householdId, CancellationToken cancellationToken)
    {
        return db.Households.AnyAsync(household => household.Id.Value == householdId, cancellationToken);
    }

    public Task<StoredFile?> GetAsync(Guid storedFileId, CancellationToken cancellationToken)
    {
        return db.StoredFiles.SingleOrDefaultAsync(item => item.Id.Value == storedFileId, cancellationToken);
    }

    public async Task<StoredFileResponse?> GetResponseAsync(Guid storedFileId, CancellationToken cancellationToken)
    {
        return await db.StoredFiles
            .AsNoTracking()
            .Where(item => item.Id.Value == storedFileId)
            .Select(item => new StoredFileResponse(
                StoredFileId: item.Id.Value,
                HouseholdId: item.HouseholdId.Value,
                StorageKey: item.StorageKey,
                FileName: item.FileName,
                ContentType: item.ContentType,
                SizeInBytes: item.SizeInBytes,
                MediaType: item.MediaType,
                Status: item.Status,
                UploadedAt: item.UploadedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public void Add(StoredFile storedFile)
    {
        db.StoredFiles.Add(storedFile);
    }
}
