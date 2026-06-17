using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Shared.Enums;
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

public interface IStoredFileQueryService
{
    Task<Result<List<StoredFileResponse>>> ListAsync(Guid householdId, CancellationToken cancellationToken);
    Task<Result<StoredFileResponse>> GetAsync(Guid storedFileId, CancellationToken cancellationToken);
}

public interface IStoredFileQueryRepository
{
    Task<List<StoredFileResponse>> ListAsync(Guid householdId, CancellationToken cancellationToken);
    Task<StoredFileResponse?> GetResponseAsync(Guid storedFileId, CancellationToken cancellationToken);
}

public sealed class StoredFileQueryService(IStoredFileQueryRepository storedFileRepo) : IStoredFileQueryService
{
    public async Task<Result<List<StoredFileResponse>>> ListAsync(Guid householdId, CancellationToken cancellationToken)
    {
        var files = await storedFileRepo.ListAsync(householdId, cancellationToken);
        return Result<List<StoredFileResponse>>.Success(files, ResultTypeEnum.Success, files.Count);
    }

    public async Task<Result<StoredFileResponse>> GetAsync(Guid storedFileId, CancellationToken cancellationToken)
    {
        var file = await storedFileRepo.GetResponseAsync(storedFileId, cancellationToken);
        return file is null
            ? Result<StoredFileResponse>.Fail("Stored file was not found.", ResultTypeEnum.NotFound)
            : Result<StoredFileResponse>.Success(file, ResultTypeEnum.Success);
    }
}
