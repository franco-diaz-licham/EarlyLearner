using EarlyLearner.Application.Common;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.PlanningContext;
using EarlyLearner.Domain.PlanningContext.Entities;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.PlanningContext;

public sealed record CreateGoalCommand(Guid ChildId, string Title, GoalTypeEnum Type, DateOnly StartDate, DateOnly EndDate, IReadOnlyList<Guid> ReadinessOutcomeIds);

public sealed record UpdateGoalCommand(Guid GoalId, string Title);

public interface IGoalCommandService
{
    Task<Result<GoalResponse>> CreateAsync(CreateGoalCommand command, CancellationToken cancellationToken);
    Task<Result<GoalResponse>> UpdateAsync(UpdateGoalCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid goalId, CancellationToken cancellationToken);
}

public interface IGoalCommandRepository
{
    Task<bool> ChildExistsAsync(Guid householdId, Guid childId, CancellationToken cancellationToken);
    Task<List<ReadinessOutcome>> GetReadinessOutcomesAsync(IReadOnlyList<Guid> readinessOutcomeIds, CancellationToken cancellationToken);
    Task<Goal?> GetAsync(Guid goalId, CancellationToken cancellationToken);
    Task<Goal?> GetWithReadinessOutcomesAsync(Guid goalId, CancellationToken cancellationToken);
    Task<GoalResponse?> GetResponseAsync(Guid goalId, CancellationToken cancellationToken);
    void Add(Goal goal);
    void Remove(Goal goal);
}

public sealed class GoalCommandService(IGoalCommandRepository goalRepo, IUnitOfWork uow, ICurrentUser currentUser) : IGoalCommandService
{
    public async Task<Result<GoalResponse>> CreateAsync(CreateGoalCommand command, CancellationToken cancellationToken)
    {
        var householdId = currentUser.HouseholdId.Value;
        var childExists = await goalRepo.ChildExistsAsync(householdId, command.ChildId, cancellationToken);
        if (!childExists) return Result<GoalResponse>.Fail("Child was not found in this household.", ResultTypeEnum.NotFound);

        var readinessOutcomes = await goalRepo.GetReadinessOutcomesAsync(command.ReadinessOutcomeIds, cancellationToken);
        if (readinessOutcomes.Count != command.ReadinessOutcomeIds.Distinct().Count()) return Result<GoalResponse>.Fail("One or more readiness outcomes were not found.", ResultTypeEnum.NotFound);

        var timeframe = DateRange.Create(command.StartDate, command.EndDate);
        var goal = Goal.Create(new HouseholdId(householdId), new ChildId(command.ChildId), command.Title, command.Type, timeframe, readinessOutcomes);

        goalRepo.Add(goal);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<GoalResponse>.Fail("Goal could not be created.", ResultTypeEnum.Invalid);

        var result = await goalRepo.GetResponseAsync(goal.Id.Value, cancellationToken);
        if (result is null) return Result<GoalResponse>.Fail("Goal could not be retrieved after creation.", ResultTypeEnum.Invalid);
        return Result<GoalResponse>.Success(result, ResultTypeEnum.Created);
    }

    public async Task<Result<GoalResponse>> UpdateAsync(UpdateGoalCommand command, CancellationToken cancellationToken)
    {
        var goal = await goalRepo.GetWithReadinessOutcomesAsync(command.GoalId, cancellationToken);
        if (goal is null) return Result<GoalResponse>.Fail("Goal was not found.", ResultTypeEnum.NotFound);

        goal.Rename(command.Title);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<GoalResponse>.Fail("Goal could not be updated.", ResultTypeEnum.Invalid);

        var result = await goalRepo.GetResponseAsync(goal.Id.Value, cancellationToken);
        if (result is null) return Result<GoalResponse>.Fail("Goal could not be retrieved after update.", ResultTypeEnum.Invalid);
        return Result<GoalResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result> DeleteAsync(Guid goalId, CancellationToken cancellationToken)
    {
        var goal = await goalRepo.GetAsync(goalId, cancellationToken);
        if (goal is null) return Result.Fail("Goal was not found.", ResultTypeEnum.NotFound);

        goalRepo.Remove(goal);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        return saved ? Result.Success(ResultTypeEnum.Success) : Result.Fail("Goal could not be deleted.", ResultTypeEnum.Invalid);
    }
}
