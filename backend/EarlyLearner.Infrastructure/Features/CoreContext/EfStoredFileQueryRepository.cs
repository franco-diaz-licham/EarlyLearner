using EarlyLearner.Application.Features.CoreContext;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.CoreContext;

public sealed class EfStoredFileQueryRepository(DatabaseContext db) : IStoredFileQueryRepository
{
    public async Task<List<StoredFileResponse>> ListAsync(Guid householdId, CancellationToken cancellationToken)
    {
        return await db.StoredFiles
            .AsNoTracking()
            .Where(file => file.HouseholdId.Value == householdId)
            .OrderByDescending(file => file.UploadedAt)
            .Select(file => new StoredFileResponse(
                StoredFileId: file.Id.Value,
                HouseholdId: file.HouseholdId.Value,
                StorageKey: file.StorageKey,
                FileName: file.FileName,
                ContentType: file.ContentType,
                SizeInBytes: file.SizeInBytes,
                MediaType: file.MediaType,
                Status: file.Status,
                UploadedAt: file.UploadedAt))
            .ToListAsync(cancellationToken);
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
}
