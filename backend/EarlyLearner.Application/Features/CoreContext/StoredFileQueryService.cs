using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.CoreContext;

public sealed record StoredFileResponse(
    Guid StoredFileId,
    Guid HouseholdId,
    string StorageKey,
    string FileName,
    string ContentType,
    long SizeInBytes,
    StoredFileMediaTypeEnum MediaType,
    StoredFileStatusEnum Status,
    DateTimeOffset UploadedAt);

public sealed record StoredFileDownloadResponse(
    Stream Content,
    string FileName,
    string ContentType);

public interface IStoredFileQueryService
{
    Task<Result<List<StoredFileResponse>>> ListAsync(CancellationToken cancellationToken);
    Task<Result<StoredFileResponse>> GetAsync(StoredFileId storedFileId, CancellationToken cancellationToken);
    Task<Result<StoredFileDownloadResponse>> DownloadAsync(StoredFileId storedFileId, CancellationToken cancellationToken);
}

public interface IStoredFileQueryRepository
{
    Task<List<StoredFileResponse>> ListAsync(HouseholdId householdId, CancellationToken cancellationToken);
    Task<StoredFileResponse?> GetResponseAsync(StoredFileId storedFileId, CancellationToken cancellationToken);
}

public sealed class StoredFileQueryService(IStoredFileQueryRepository storedFileRepo, ICurrentUser currentUser, IFileStorageService fileStorageService) : IStoredFileQueryService
{
    public async Task<Result<List<StoredFileResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var files = await storedFileRepo.ListAsync(currentUser.HouseholdId, cancellationToken);
        return Result<List<StoredFileResponse>>.Success(files, ResultTypeEnum.Success, files.Count);
    }

    public async Task<Result<StoredFileResponse>> GetAsync(StoredFileId storedFileId, CancellationToken cancellationToken)
    {
        var file = await storedFileRepo.GetResponseAsync(storedFileId, cancellationToken);
        return file is null || new HouseholdId(file.HouseholdId) != currentUser.HouseholdId
            ? Result<StoredFileResponse>.Fail("Stored file was not found.", ResultTypeEnum.NotFound)
            : Result<StoredFileResponse>.Success(file, ResultTypeEnum.Success);
    }

    public async Task<Result<StoredFileDownloadResponse>> DownloadAsync(StoredFileId storedFileId, CancellationToken cancellationToken)
    {
        var fileResult = await GetAsync(storedFileId, cancellationToken);
        if (!fileResult.IsSuccess || fileResult.Value is null) return Result<StoredFileDownloadResponse>.Fail(fileResult.Error?.Message ?? "Stored file was not found.", fileResult.Type);

        var file = fileResult.Value;
        var stream = await fileStorageService.DownloadAsync(file.StorageKey, cancellationToken);
        var download = new StoredFileDownloadResponse(stream, file.FileName, file.ContentType);
        return Result<StoredFileDownloadResponse>.Success(download, ResultTypeEnum.Success);
    }
}
