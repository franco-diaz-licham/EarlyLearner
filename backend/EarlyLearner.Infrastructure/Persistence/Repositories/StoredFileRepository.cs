using EarlyLearner.Application.UseCases.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Persistence.Repositories;

public sealed class StoredFileRepository(DatabaseContext db) : IStoredFileQueryRepository, IStoredFileCommandRepository
{
    public async Task<List<StoredFileResponse>> ListAsync(HouseholdId householdId, CancellationToken cancellationToken)
    {
        return await db.StoredFiles
            .AsNoTracking()
            .Where(file => file.HouseholdId == householdId)
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

    public Task<StoredFile?> GetAsync(StoredFileId storedFileId, CancellationToken cancellationToken)
    {
        return db.StoredFiles.SingleOrDefaultAsync(item => item.Id == storedFileId, cancellationToken);
    }

    public async Task<StoredFileResponse?> GetResponseAsync(StoredFileId storedFileId, CancellationToken cancellationToken)
    {
        return await db.StoredFiles
            .AsNoTracking()
            .Where(item => item.Id == storedFileId)
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
