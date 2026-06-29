using EarlyLearner.Application.Ports;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.PlanningContext;

public sealed record LearningPlanResponse(Guid LearningPlanId, Guid HouseholdId, Guid ChildId, DateOnly StartDate, DateOnly EndDate, string Focus);

public interface ILearningPlanQueryService
{
    Task<Result<List<LearningPlanResponse>>> ListAsync(CancellationToken cancellationToken);
    Task<Result<LearningPlanResponse>> GetAsync(Guid learningPlanId, CancellationToken cancellationToken);
}

public interface ILearningPlanQueryRepository
{
    Task<List<LearningPlanResponse>> ListAsync(Guid householdId, CancellationToken cancellationToken);
    Task<LearningPlanResponse?> GetResponseAsync(Guid learningPlanId, CancellationToken cancellationToken);
}

public sealed class LearningPlanQueryService(ILearningPlanQueryRepository learningPlanRepo, ICurrentUser currentUser) : ILearningPlanQueryService
{
    public async Task<Result<List<LearningPlanResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var plans = await learningPlanRepo.ListAsync(currentUser.HouseholdId.Value, cancellationToken);
        return Result<List<LearningPlanResponse>>.Success(plans, ResultTypeEnum.Success, plans.Count);
    }

    public async Task<Result<LearningPlanResponse>> GetAsync(Guid learningPlanId, CancellationToken cancellationToken)
    {
        var plan = await learningPlanRepo.GetResponseAsync(learningPlanId, cancellationToken);
        return plan is null
            ? Result<LearningPlanResponse>.Fail("Learning plan was not found.", ResultTypeEnum.NotFound)
            : Result<LearningPlanResponse>.Success(plan, ResultTypeEnum.Success);
    }
}
