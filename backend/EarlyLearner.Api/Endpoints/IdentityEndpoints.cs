using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.IdentityContext;
using Microsoft.AspNetCore.Mvc;

namespace EarlyLearner.Api.Endpoints;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var households = endpoints.MapGroup("/households").WithTags("Households");

        households.MapGet("/", async (IHouseholdQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.ListAsync(cancellationToken)).ToApiResult());

        households.MapGet("/{householdId:guid}", async (Guid householdId, IHouseholdQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.GetAsync(householdId, cancellationToken)).ToApiResult());

        households.MapPost("/", async ([FromBody] CreateHouseholdCommand command, IHouseholdCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.CreateAsync(command, cancellationToken)).ToApiResult());

        households.MapPut("/{householdId:guid}", async (Guid householdId, [FromBody] UpdateHouseholdRequest request, IHouseholdCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.UpdateAsync(new UpdateHouseholdCommand(householdId, request.Name), cancellationToken)).ToApiResult());

        households.MapDelete("/{householdId:guid}", async (Guid householdId, IHouseholdCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.DeleteAsync(householdId, cancellationToken)).ToApiResult());

        return endpoints;
    }
}

public sealed record UpdateHouseholdRequest(string Name);
