using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.PlanningContext;
using Microsoft.AspNetCore.Mvc;

namespace EarlyLearner.Api.Endpoints;

public static class PlanningEndpoints
{
    public static IEndpointRouteBuilder MapPlanningEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var goals = endpoints.MapGroup("/goals").WithTags("Goals");

        goals.MapGet("/", async (Guid householdId, IGoalQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.ListAsync(householdId, cancellationToken)).ToApiResult());

        goals.MapGet("/{goalId:guid}", async (Guid goalId, IGoalQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.GetAsync(goalId, cancellationToken)).ToApiResult());

        goals.MapPost("/", async ([FromBody] CreateGoalCommand command, IGoalCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.CreateAsync(command, cancellationToken)).ToApiResult());

        goals.MapPut("/{goalId:guid}", async (Guid goalId, [FromBody] UpdateGoalRequest request, IGoalCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.UpdateAsync(new UpdateGoalCommand(goalId, request.Title), cancellationToken)).ToApiResult());

        goals.MapDelete("/{goalId:guid}", async (Guid goalId, IGoalCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.DeleteAsync(goalId, cancellationToken)).ToApiResult());

        var learningPlans = endpoints.MapGroup("/learning-plans").WithTags("Learning plans");

        learningPlans.MapGet("/", async (Guid householdId, ILearningPlanQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.ListAsync(householdId, cancellationToken)).ToApiResult());

        learningPlans.MapGet("/{learningPlanId:guid}", async (Guid learningPlanId, ILearningPlanQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.GetAsync(learningPlanId, cancellationToken)).ToApiResult());

        learningPlans.MapPost("/", async ([FromBody] CreateLearningPlanCommand command, ILearningPlanCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.CreateAsync(command, cancellationToken)).ToApiResult());

        learningPlans.MapPut("/{learningPlanId:guid}", async (Guid learningPlanId, [FromBody] UpdateLearningPlanRequest request, ILearningPlanCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.UpdateAsync(new UpdateLearningPlanCommand(learningPlanId, request.StartDate, request.EndDate, request.Focus), cancellationToken)).ToApiResult());

        learningPlans.MapDelete("/{learningPlanId:guid}", async (Guid learningPlanId, ILearningPlanCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.DeleteAsync(learningPlanId, cancellationToken)).ToApiResult());

        return endpoints;
    }
}

public sealed record UpdateGoalRequest(string Title);

public sealed record UpdateLearningPlanRequest(DateOnly StartDate, DateOnly EndDate, string Focus);
