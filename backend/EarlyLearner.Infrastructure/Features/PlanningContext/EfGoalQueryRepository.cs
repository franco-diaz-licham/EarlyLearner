using EarlyLearner.Application.Features.PlanningContext;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.PlanningContext;

public sealed class EfGoalQueryRepository(DatabaseContext db) : IGoalQueryRepository
{
    public async Task<List<GoalResponse>> ListAsync(Guid householdId, CancellationToken cancellationToken)
    {
        return await db.Goals
            .AsNoTracking()
            .Where(goal => goal.HouseholdId.Value == householdId)
            .OrderBy(goal => goal.ChildId.Value)
            .ThenBy(goal => goal.Title)
            .Select(goal => new GoalResponse(
                GoalId: goal.Id.Value,
                HouseholdId: goal.HouseholdId.Value,
                ChildId: goal.ChildId.Value,
                Title: goal.Title,
                Type: goal.Type,
                Status: goal.Status,
                StartDate: goal.Timeframe.StartDate,
                EndDate: goal.Timeframe.EndDate,
                ReadinessOutcomeIds: goal.ReadinessOutcomes.Select(outcome => outcome.Id.Value).ToList()))
            .ToListAsync(cancellationToken);
    }

    public async Task<GoalResponse?> GetResponseAsync(Guid goalId, CancellationToken cancellationToken)
    {
        return await db.Goals
            .AsNoTracking()
            .Where(item => item.Id.Value == goalId)
            .Select(item => new GoalResponse(
                GoalId: item.Id.Value,
                HouseholdId: item.HouseholdId.Value,
                ChildId: item.ChildId.Value,
                Title: item.Title,
                Type: item.Type,
                Status: item.Status,
                StartDate: item.Timeframe.StartDate,
                EndDate: item.Timeframe.EndDate,
                ReadinessOutcomeIds: item.ReadinessOutcomes.Select(outcome => outcome.Id.Value).ToList()))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
