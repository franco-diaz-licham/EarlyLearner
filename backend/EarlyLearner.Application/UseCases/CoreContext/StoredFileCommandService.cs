using EarlyLearner.Application.Common;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using System.Net.Mime;

namespace EarlyLearner.Application.UseCases.CoreContext;

public sealed record CreateStoredFileCommand(
    Stream Content,
    string FileName,
    string ContentType,
    long SizeInBytes,
    StoredFileMediaTypeEnum MediaType,
    DateTimeOffset UploadedAt,
    string? StorageKey = null);

public sealed record UpdateStoredFileStatusCommand(StoredFileId StoredFileId, StoredFileStatusEnum Status);

public interface IStoredFileCommandService
{
    Task<Result<StoredFileResponse>> CreateAsync(CreateStoredFileCommand command, CancellationToken cancellationToken);
    Task<Result<StoredFileResponse>> UpdateStatusAsync(UpdateStoredFileStatusCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(StoredFileId storedFileId, CancellationToken cancellationToken);
}

public interface IStoredFileCommandRepository
{
    Task<StoredFile?> GetAsync(StoredFileId storedFileId, CancellationToken cancellationToken);
    Task<StoredFileResponse?> GetResponseAsync(StoredFileId storedFileId, CancellationToken cancellationToken);
    void Add(StoredFile storedFile);
}

public sealed class StoredFileCommandService(IStoredFileCommandRepository storedFileRepo, IUnitOfWork uow, ICurrentUser currentUser, IFileStorageService fileStorageService) : IStoredFileCommandService
{
    public async Task<Result<StoredFileResponse>> CreateAsync(CreateStoredFileCommand command, CancellationToken cancellationToken)
    {
        var storageKey = string.IsNullOrWhiteSpace(command.StorageKey) ? CreateStorageKey(command.FileName) : command.StorageKey;
        await fileStorageService.UploadAsync(storageKey, new ContentType(command.ContentType), command.Content, cancellationToken);

        var file = StoredFile.Create(
            currentUser.HouseholdId,
            storageKey,
            command.FileName,
            command.ContentType,
            command.SizeInBytes,
            command.MediaType,
            command.UploadedAt);

        storedFileRepo.Add(file);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<StoredFileResponse>.Fail("Stored file could not be created.", ResultTypeEnum.Invalid);

        var result = await storedFileRepo.GetResponseAsync(file.Id, cancellationToken);
        if (result is null) return Result<StoredFileResponse>.Fail("Stored file could not be retrieved after creation.", ResultTypeEnum.Invalid);
        return Result<StoredFileResponse>.Success(result, ResultTypeEnum.Created);
    }

    private static string CreateStorageKey(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return $"uploads/{Guid.NewGuid():N}{extension}";
    }

    public async Task<Result<StoredFileResponse>> UpdateStatusAsync(UpdateStoredFileStatusCommand command, CancellationToken cancellationToken)
    {
        var file = await storedFileRepo.GetAsync(command.StoredFileId, cancellationToken);
        if (file is null) return Result<StoredFileResponse>.Fail("Stored file was not found.", ResultTypeEnum.NotFound);

        switch (command.Status) {
            case StoredFileStatusEnum.Available:
                file.MarkAvailable();
                break;
            case StoredFileStatusEnum.Rejected:
                file.Reject();
                break;
            case StoredFileStatusEnum.Deleted:
                file.Delete();
                break;
            default:
                return Result<StoredFileResponse>.Fail("Stored file status cannot be changed to the requested value.", ResultTypeEnum.Invalid);
        }

        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<StoredFileResponse>.Fail("Stored file could not be updated.", ResultTypeEnum.Invalid);

        var result = await storedFileRepo.GetResponseAsync(file.Id, cancellationToken);
        if (result is null) return Result<StoredFileResponse>.Fail("Stored file could not be retrieved after update.", ResultTypeEnum.Invalid);
        return Result<StoredFileResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result> DeleteAsync(StoredFileId storedFileId, CancellationToken cancellationToken)
    {
        var file = await storedFileRepo.GetAsync(storedFileId, cancellationToken);
        if (file is null) return Result.Fail("Stored file was not found.", ResultTypeEnum.NotFound);

        file.Delete();
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        return saved ? Result.Success(ResultTypeEnum.Success) : Result.Fail("Stored file could not be deleted.", ResultTypeEnum.Invalid);
    }
}
