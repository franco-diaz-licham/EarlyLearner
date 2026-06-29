using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.PlanningContext;
using EarlyLearner.Domain.PlanningContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.PlanningContext;

public sealed record GoalResponse(Guid GoalId, Guid HouseholdId, Guid ChildId, string Title, GoalTypeEnum Type, GoalStatusEnum Status, DateOnly StartDate, DateOnly EndDate, IReadOnlyList<Guid> ReadinessOutcomeIds);

public interface IGoalQueryService
{
    Task<Result<List<GoalResponse>>> ListAsync(CancellationToken cancellationToken);
    Task<Result<GoalResponse>> GetAsync(GoalId goalId, CancellationToken cancellationToken);
}

public interface IGoalQueryRepository
{
    Task<List<GoalResponse>> ListAsync(HouseholdId householdId, CancellationToken cancellationToken);
    Task<GoalResponse?> GetResponseAsync(GoalId goalId, CancellationToken cancellationToken);
}

public sealed class GoalQueryService(IGoalQueryRepository goalRepo, ICurrentUser currentUser) : IGoalQueryService
{
    public async Task<Result<List<GoalResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var goals = await goalRepo.ListAsync(currentUser.HouseholdId, cancellationToken);
        return Result<List<GoalResponse>>.Success(goals, ResultTypeEnum.Success, goals.Count);
    }

    public async Task<Result<GoalResponse>> GetAsync(GoalId goalId, CancellationToken cancellationToken)
    {
        var goal = await goalRepo.GetResponseAsync(goalId, cancellationToken);
        return goal is null
            ? Result<GoalResponse>.Fail("Goal was not found.", ResultTypeEnum.NotFound)
            : Result<GoalResponse>.Success(goal, ResultTypeEnum.Success);
    }
}
