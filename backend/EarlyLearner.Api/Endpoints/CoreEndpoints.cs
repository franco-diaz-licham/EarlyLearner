using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.CoreContext;
using EarlyLearner.Domain.CoreContext;
using Microsoft.AspNetCore.Mvc;

namespace EarlyLearner.Api.Endpoints;

public static class CoreEndpoints
{
    public static IEndpointRouteBuilder MapCoreEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var files = endpoints.MapGroup("/stored-files").WithTags("Stored files");

        files.MapGet("/", async (Guid householdId, IStoredFileQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.ListAsync(householdId, cancellationToken)).ToApiResult());

        files.MapGet("/{storedFileId:guid}", async (Guid storedFileId, IStoredFileQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.GetAsync(storedFileId, cancellationToken)).ToApiResult());

        files.MapPost("/", async ([FromBody] CreateStoredFileCommand command, IStoredFileCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.CreateAsync(command, cancellationToken)).ToApiResult());

        files.MapPut("/{storedFileId:guid}/status", async (Guid storedFileId, [FromBody] UpdateStoredFileStatusRequest request, IStoredFileCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.UpdateStatusAsync(new UpdateStoredFileStatusCommand(storedFileId, request.Status), cancellationToken)).ToApiResult());

        files.MapDelete("/{storedFileId:guid}", async (Guid storedFileId, IStoredFileCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.DeleteAsync(storedFileId, cancellationToken)).ToApiResult());

        return endpoints;
    }
}

public sealed record UpdateStoredFileStatusRequest(StoredFileStatusEnum Status);
