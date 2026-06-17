using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.PlanningContext;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;
using FluentValidation;

namespace EarlyLearner.Api.Endpoints;

public static class LearningPlanEndpoints
{
    public static IEndpointRouteBuilder MapLearningPlanEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var learningPlans = endpoints.MapGroup("/learning-plans").WithTags("Learning plans");

        learningPlans.MapGet("/", ListLearningPlans).WithName(nameof(ListLearningPlans));
        learningPlans.MapGet("/{learningPlanId:guid}", GetLearningPlan).WithName(nameof(GetLearningPlan));
        learningPlans.MapPost("/", CreateLearningPlan).WithName(nameof(CreateLearningPlan));
        learningPlans.MapPut("/{learningPlanId:guid}", UpdateLearningPlan).WithName(nameof(UpdateLearningPlan));
        learningPlans.MapDelete("/{learningPlanId:guid}", DeleteLearningPlan).WithName(nameof(DeleteLearningPlan));

        return endpoints;
    }

    public static async Task<IResult> ListLearningPlans(Guid householdId, ILearningPlanQueryService queryService, CancellationToken cancellationToken = default)
    {
        if (householdId == Guid.Empty) return Result<List<LearningPlanResponse>>.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await queryService.ListAsync(householdId, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> GetLearningPlan(Guid learningPlanId, ILearningPlanQueryService queryService, CancellationToken cancellationToken = default)
    {
        if (learningPlanId == Guid.Empty) return Result<LearningPlanResponse>.Fail("Learning plan id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await queryService.GetAsync(learningPlanId, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> CreateLearningPlan(CreateLearningPlanRequest request, IValidator<CreateLearningPlanRequest> validator, ILearningPlanCommandService commandService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new CreateLearningPlanCommand(
            HouseholdId: request.HouseholdId,
            ChildId: request.ChildId,
            StartDate: request.StartDate,
            EndDate: request.EndDate,
            Focus: request.Focus);

        var result = await commandService.CreateAsync(command, cancellationToken);
        var locationUrl = result.IsSuccess ? $"/learning-plans/{result.Value.LearningPlanId}" : null;
        return result.ToApiResult(locationUrl);
    }

    public static async Task<IResult> UpdateLearningPlan(Guid learningPlanId, UpdateLearningPlanRequest request, IValidator<UpdateLearningPlanRequest> validator, ILearningPlanCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (learningPlanId == Guid.Empty) return Result<LearningPlanResponse>.Fail("Learning plan id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new UpdateLearningPlanCommand(
            LearningPlanId: learningPlanId,
            StartDate: request.StartDate,
            EndDate: request.EndDate,
            Focus: request.Focus);

        var result = await commandService.UpdateAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> DeleteLearningPlan(Guid learningPlanId, ILearningPlanCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (learningPlanId == Guid.Empty) return Result.Fail("Learning plan id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await commandService.DeleteAsync(learningPlanId, cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record CreateLearningPlanRequest(Guid HouseholdId, Guid ChildId, DateOnly StartDate, DateOnly EndDate, string Focus);

public sealed class CreateLearningPlanRequestValidator : AbstractValidator<CreateLearningPlanRequest>
{
    public CreateLearningPlanRequestValidator()
    {
        RuleFor(request => request.HouseholdId).NotEmpty();
        RuleFor(request => request.ChildId).NotEmpty();
        RuleFor(request => request.StartDate).NotEqual(default(DateOnly));
        RuleFor(request => request.EndDate).NotEqual(default(DateOnly));
        RuleFor(request => request.EndDate).GreaterThanOrEqualTo(request => request.StartDate);
        RuleFor(request => request.Focus).NotEmpty();
    }
}

public sealed record UpdateLearningPlanRequest(DateOnly StartDate, DateOnly EndDate, string Focus);

public sealed class UpdateLearningPlanRequestValidator : AbstractValidator<UpdateLearningPlanRequest>
{
    public UpdateLearningPlanRequestValidator()
    {
        RuleFor(request => request.StartDate).NotEqual(default(DateOnly));
        RuleFor(request => request.EndDate).NotEqual(default(DateOnly));
        RuleFor(request => request.EndDate).GreaterThanOrEqualTo(request => request.StartDate);
        RuleFor(request => request.Focus).NotEmpty();
    }
}
