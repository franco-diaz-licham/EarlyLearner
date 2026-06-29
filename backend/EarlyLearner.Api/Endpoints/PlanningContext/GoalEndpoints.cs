using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.PlanningContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.PlanningContext;
using EarlyLearner.Domain.PlanningContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;
using FluentValidation;

namespace EarlyLearner.Api.Endpoints;

public static class GoalEndpoints
{
    public static IEndpointRouteBuilder MapGoalEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var goals = endpoints.MapGroup("/goals").WithTags("Goals");

        goals.MapGet("/", ListGoals).WithName(nameof(ListGoals));
        goals.MapGet("/{goalId:guid}", GetGoal).WithName(nameof(GetGoal));
        goals.MapPost("/", CreateGoal).WithName(nameof(CreateGoal));
        goals.MapPut("/{goalId:guid}", UpdateGoal).WithName(nameof(UpdateGoal));
        goals.MapDelete("/{goalId:guid}", DeleteGoal).WithName(nameof(DeleteGoal));

        return endpoints;
    }

    public static async Task<IResult> ListGoals(IGoalQueryService queryService, CancellationToken cancellationToken = default)
    {
        var result = await queryService.ListAsync(cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> GetGoal(Guid goalId, IGoalQueryService queryService, CancellationToken cancellationToken = default)
    {
        if (goalId == Guid.Empty) return Result<GoalResponse>.Fail("Goal id is required.", ResultTypeEnum.Invalid).ToApiResult();
        var result = await queryService.GetAsync(new GoalId(goalId), cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> CreateGoal(CreateGoalRequest request, IValidator<CreateGoalRequest> validator, IGoalCommandService commandService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new CreateGoalCommand(
            ChildId: new ChildId(request.ChildId),
            Title: request.Title,
            Type: request.Type,
            StartDate: request.StartDate,
            EndDate: request.EndDate,
            ReadinessOutcomeIds: request.ReadinessOutcomeIds.Select(id => new ReadinessOutcomeId(id)).ToList());

        var result = await commandService.CreateAsync(command, cancellationToken);
        var locationUrl = result.IsSuccess ? $"/goals/{result.Value.GoalId}" : null;
        return result.ToApiResult(locationUrl);
    }

    public static async Task<IResult> UpdateGoal(Guid goalId, UpdateGoalRequest request, IValidator<UpdateGoalRequest> validator, IGoalCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (goalId == Guid.Empty) return Result<GoalResponse>.Fail("Goal id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new UpdateGoalCommand(GoalId: new GoalId(goalId), Title: request.Title);
        var result = await commandService.UpdateAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> DeleteGoal(Guid goalId, IGoalCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (goalId == Guid.Empty) return Result.Fail("Goal id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await commandService.DeleteAsync(new GoalId(goalId), cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record CreateGoalRequest(Guid ChildId, string Title, GoalTypeEnum Type, DateOnly StartDate, DateOnly EndDate, IReadOnlyList<Guid> ReadinessOutcomeIds);

public sealed class CreateGoalRequestValidator : AbstractValidator<CreateGoalRequest>
{
    public CreateGoalRequestValidator()
    {
        RuleFor(request => request.ChildId).NotEmpty();
        RuleFor(request => request.Title).NotEmpty();
        RuleFor(request => request.Type).IsInEnum();
        RuleFor(request => request.StartDate).NotEqual(default(DateOnly));
        RuleFor(request => request.EndDate).NotEqual(default(DateOnly));
        RuleFor(request => request.EndDate).GreaterThanOrEqualTo(request => request.StartDate);
        RuleFor(request => request.ReadinessOutcomeIds).NotNull();
    }
}

public sealed record UpdateGoalRequest(string Title);

public sealed class UpdateGoalRequestValidator : AbstractValidator<UpdateGoalRequest>
{
    public UpdateGoalRequestValidator()
    {
        RuleFor(request => request.Title).NotEmpty();
    }
}
