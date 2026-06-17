using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.ReadinessContext;
using Microsoft.AspNetCore.Mvc;

namespace EarlyLearner.Api.Endpoints;

public static class ReadinessEndpoints
{
    public static IEndpointRouteBuilder MapReadinessEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var outcomes = endpoints.MapGroup("/readiness-outcomes").WithTags("Readiness outcomes");

        outcomes.MapGet("/", async (IReadinessOutcomeQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.ListAsync(cancellationToken)).ToApiResult());

        outcomes.MapGet("/{readinessOutcomeId:guid}", async (Guid readinessOutcomeId, IReadinessOutcomeQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.GetAsync(readinessOutcomeId, cancellationToken)).ToApiResult());

        outcomes.MapPost("/", async ([FromBody] CreateReadinessOutcomeCommand command, IReadinessOutcomeCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.CreateAsync(command, cancellationToken)).ToApiResult());

        outcomes.MapPut("/{readinessOutcomeId:guid}", async (Guid readinessOutcomeId, [FromBody] UpdateReadinessOutcomeRequest request, IReadinessOutcomeCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.UpdateAsync(new UpdateReadinessOutcomeCommand(readinessOutcomeId, request.Name, request.Description, request.Category, request.SortOrder), cancellationToken)).ToApiResult());

        outcomes.MapDelete("/{readinessOutcomeId:guid}", async (Guid readinessOutcomeId, IReadinessOutcomeCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.DeleteAsync(readinessOutcomeId, cancellationToken)).ToApiResult());

        var profiles = endpoints.MapGroup("/readiness-profiles").WithTags("Readiness profiles");

        profiles.MapGet("/", async (Guid householdId, IReadinessProfileQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.ListAsync(householdId, cancellationToken)).ToApiResult());

        profiles.MapGet("/{readinessProfileId:guid}", async (Guid readinessProfileId, IReadinessProfileQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.GetAsync(readinessProfileId, cancellationToken)).ToApiResult());

        profiles.MapPost("/", async ([FromBody] CreateReadinessProfileCommand command, IReadinessProfileCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.CreateAsync(command, cancellationToken)).ToApiResult());

        profiles.MapDelete("/{readinessProfileId:guid}", async (Guid readinessProfileId, IReadinessProfileCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.DeleteAsync(readinessProfileId, cancellationToken)).ToApiResult());

        return endpoints;
    }
}

public sealed record UpdateReadinessOutcomeRequest(string Name, string Description, string Category, int SortOrder);
