using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.CoreContext;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;
using FluentValidation;

namespace EarlyLearner.Api.Endpoints;

public static class StoredFileEndpoints
{
    public static IEndpointRouteBuilder MapStoredFileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var files = endpoints.MapGroup("/stored-files").WithTags("Stored files");

        files.MapGet("/", ListStoredFiles).WithName(nameof(ListStoredFiles));
        files.MapGet("/{storedFileId:guid}", GetStoredFile).WithName(nameof(GetStoredFile));
        files.MapPost("/", CreateStoredFile).WithName(nameof(CreateStoredFile));
        files.MapPut("/{storedFileId:guid}/status", UpdateStoredFileStatus).WithName(nameof(UpdateStoredFileStatus));
        files.MapDelete("/{storedFileId:guid}", DeleteStoredFile).WithName(nameof(DeleteStoredFile));

        return endpoints;
    }

    public static async Task<IResult> ListStoredFiles(Guid householdId, IStoredFileQueryService queryService, CancellationToken cancellationToken = default)
    {
        if (householdId == Guid.Empty) return Result<List<StoredFileResponse>>.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await queryService.ListAsync(householdId, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> GetStoredFile(Guid storedFileId, IStoredFileQueryService queryService, CancellationToken cancellationToken = default)
    {
        if (storedFileId == Guid.Empty) return Result<StoredFileResponse>.Fail("Stored file id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await queryService.GetAsync(storedFileId, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> CreateStoredFile(CreateStoredFileRequest request, IValidator<CreateStoredFileRequest> validator, IStoredFileCommandService commandService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new CreateStoredFileCommand(
            HouseholdId: request.HouseholdId,
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

    public static async Task<IResult> UpdateStoredFileStatus(Guid storedFileId, UpdateStoredFileStatusRequest request, IValidator<UpdateStoredFileStatusRequest> validator, IStoredFileCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (storedFileId == Guid.Empty) return Result<StoredFileResponse>.Fail("Stored file id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new UpdateStoredFileStatusCommand(
            StoredFileId: storedFileId,
            Status: request.Status);

        var result = await commandService.UpdateStatusAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> DeleteStoredFile(Guid storedFileId, IStoredFileCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (storedFileId == Guid.Empty) return Result.Fail("Stored file id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await commandService.DeleteAsync(storedFileId, cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record CreateStoredFileRequest(
    Guid HouseholdId,
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
        RuleFor(request => request.HouseholdId).NotEmpty();
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
