using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.UseCases.CoreContext;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.ValueObjects;
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
        files.MapPost("/", CreateStoredFile).WithName(nameof(CreateStoredFile)).DisableAntiforgery();
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

        var result = await queryService.DownloadAsync(new StoredFileId(storedFileId), cancellationToken);
        if (!result.IsSuccess || result.Value is null) return result.ToApiResult();

        var storedFile = result.Value;
        return Results.File(storedFile.Content, storedFile.ContentType, storedFile.FileName);
    }

    public static async Task<IResult> CreateStoredFile([AsParameters] CreateStoredFileRequest request, IValidator<CreateStoredFileRequest> validator, IStoredFileCommandService commandService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var file = request.File!;
        await using var stream = file.OpenReadStream();
        Enum.TryParse<StoredFileMediaTypeEnum>(request.MediaType, ignoreCase: true, out var mediaType);

        var command = new CreateStoredFileCommand(
            Content: stream,
            FileName: file.FileName,
            ContentType: string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
            SizeInBytes: file.Length,
            MediaType: mediaType,
            UploadedAt: request.UploadedAt ?? DateTimeOffset.UtcNow,
            StorageKey: request.StorageKey);

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
    [FromForm] IFormFile File,
    [FromForm] string MediaType,
    [FromForm] DateTimeOffset? UploadedAt,
    [FromForm] string? StorageKey = null);

public sealed class CreateStoredFileRequestValidator : AbstractValidator<CreateStoredFileRequest>
{
    public CreateStoredFileRequestValidator()
    {
        RuleFor(request => request.File).NotNull().WithMessage("File is required.");
        RuleFor(request => request.File!.FileName).NotEmpty().When(request => request.File is not null);
        RuleFor(request => request.File!.Length).GreaterThan(0).When(request => request.File is not null);
        RuleFor(request => request.MediaType)
            .NotEmpty()
            .Must(value => Enum.TryParse<StoredFileMediaTypeEnum>(value, ignoreCase: true, out _))
            .WithMessage("Media type must be one of: photo, video, document, artwork.");
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
