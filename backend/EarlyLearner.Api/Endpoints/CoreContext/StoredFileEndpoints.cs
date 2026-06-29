using Azure.Storage.Blobs;
using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.CoreContext;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EarlyLearner.Api.Endpoints;

public static class StoredFileEndpoints
{
    public static IEndpointRouteBuilder MapStoredFileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var files = endpoints.MapGroup("/stored-files").WithTags("Stored files");

        files.MapGet("/", ListStoredFiles).WithName(nameof(ListStoredFiles));
        files.MapGet("/{storedFileId:guid}", GetStoredFile).WithName(nameof(GetStoredFile));
        files.MapGet("/{storedFileId:guid}/content", GetStoredFileContent).WithName(nameof(GetStoredFileContent));
        files.MapPost("/", CreateStoredFile).WithName(nameof(CreateStoredFile));
        files.MapPut("/{storedFileId:guid}/status", UpdateStoredFileStatus).WithName(nameof(UpdateStoredFileStatus));
        files.MapDelete("/{storedFileId:guid}", DeleteStoredFile).WithName(nameof(DeleteStoredFile));

        return endpoints;
    }

    public static async Task<IResult> ListStoredFiles(IStoredFileQueryService queryService, CancellationToken cancellationToken = default)
    {
        var result = await queryService.ListAsync(cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> GetStoredFile(Guid storedFileId, IValidator<StoredFileRequest> validator, IStoredFileQueryService queryService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(new StoredFileRequest(storedFileId)).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var result = await queryService.GetAsync(new StoredFileId(storedFileId), cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> GetStoredFileContent(Guid storedFileId, IValidator<StoredFileRequest> validator, IStoredFileQueryService queryService, BlobContainerClient blobContainerClient, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(new StoredFileRequest(storedFileId)).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var result = await queryService.GetAsync(new StoredFileId(storedFileId), cancellationToken);
        if (!result.IsSuccess || result.Value is null) return result.ToApiResult();

        var storedFile = result.Value;
        var blobClient = blobContainerClient.GetBlobClient(storedFile.StorageKey);
        if (!await blobClient.ExistsAsync(cancellationToken)) return Result.Fail("Stored file content was not found.", ResultTypeEnum.NotFound).ToApiResult();

        var stream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        return Results.File(stream, storedFile.ContentType, storedFile.FileName);
    }

    public static async Task<IResult> CreateStoredFile(CreateStoredFileRequest request, IValidator<CreateStoredFileRequest> validator, IStoredFileCommandService commandService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new CreateStoredFileCommand(
            StorageKey: request.StorageKey,
            FileName: request.FileName,
            ContentType: request.ContentType,
            SizeInBytes: request.SizeInBytes,
            MediaType: request.MediaType,
            UploadedAt: request.UploadedAt);

        var result = await commandService.CreateAsync(command, cancellationToken);
        var locationUrl = result.IsSuccess ? $"/stored-files/{result.Value.StoredFileId}" : null;
        return result.ToApiResult(locationUrl);
    }

    public static async Task<IResult> UpdateStoredFileStatus(Guid storedFileId, UpdateStoredFileStatusRequest request, IValidator<StoredFileRequest> storedFileValidator, IValidator<UpdateStoredFileStatusRequest> validator, IStoredFileCommandService commandService, CancellationToken cancellationToken = default)
    {
        var storedFileValidation = storedFileValidator.Validate(new StoredFileRequest(storedFileId)).ToResult();
        if (!storedFileValidation.IsSuccess) return storedFileValidation.ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new UpdateStoredFileStatusCommand(StoredFileId: new StoredFileId(storedFileId), Status: request.Status);
        var result = await commandService.UpdateStatusAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> DeleteStoredFile(Guid storedFileId, IValidator<StoredFileRequest> validator, IStoredFileCommandService commandService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(new StoredFileRequest(storedFileId)).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var result = await commandService.DeleteAsync(new StoredFileId(storedFileId), cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record StoredFileRequest(Guid StoredFileId);

public sealed class StoredFileRequestValidator : AbstractValidator<StoredFileRequest>
{
    public StoredFileRequestValidator()
    {
        RuleFor(request => request.StoredFileId).NotEmpty().WithMessage("Stored file id is required.");
    }
}

public sealed record CreateStoredFileRequest(
    string StorageKey,
    string FileName,
    string ContentType,
    long SizeInBytes,
    StoredFileMediaTypeEnum MediaType,
    DateTimeOffset UploadedAt);

public sealed class CreateStoredFileRequestValidator : AbstractValidator<CreateStoredFileRequest>
{
    public CreateStoredFileRequestValidator()
    {
        RuleFor(request => request.StorageKey).NotEmpty();
        RuleFor(request => request.FileName).NotEmpty();
        RuleFor(request => request.ContentType).NotEmpty();
        RuleFor(request => request.SizeInBytes).GreaterThan(0);
        RuleFor(request => request.MediaType).IsInEnum();
        RuleFor(request => request.UploadedAt).NotEqual(default(DateTimeOffset));
    }
}

public sealed record UpdateStoredFileStatusRequest(StoredFileStatusEnum Status);

public sealed class UpdateStoredFileStatusRequestValidator : AbstractValidator<UpdateStoredFileStatusRequest>
{
    public UpdateStoredFileStatusRequestValidator()
    {
        RuleFor(request => request.Status).IsInEnum();
    }
}
