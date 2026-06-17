using EarlyLearner.Application.Features.PlanningContext;
using EarlyLearner.Domain.PlanningContext.Entities;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.PlanningContext;

public sealed class GoalRepository(DatabaseContext db) : IGoalQueryRepository, IGoalCommandRepository
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

    public Task<bool> ChildExistsAsync(Guid householdId, Guid childId, CancellationToken cancellationToken)
    {
        return db.Children.AnyAsync(child => child.Id.Value == childId && child.HouseholdId.Value == householdId, cancellationToken);
    }

    public async Task<List<ReadinessOutcome>> GetReadinessOutcomesAsync(IReadOnlyList<Guid> readinessOutcomeIds, CancellationToken cancellationToken)
    {
        var ids = readinessOutcomeIds.Distinct().Select(id => new ReadinessOutcomeId(id)).ToArray();
        return await db.ReadinessOutcomes
            .Where(outcome => ids.Contains(outcome.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<Goal?> GetAsync(Guid goalId, CancellationToken cancellationToken)
    {
        return db.Goals.SingleOrDefaultAsync(item => item.Id.Value == goalId, cancellationToken);
    }

    public Task<Goal?> GetWithReadinessOutcomesAsync(Guid goalId, CancellationToken cancellationToken)
    {
        return db.Goals
            .Include(item => item.ReadinessOutcomes)
            .SingleOrDefaultAsync(item => item.Id.Value == goalId, cancellationToken);
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

    public void Add(Goal goal)
    {
        db.Goals.Add(goal);
    }

    public void Remove(Goal goal)
    {
        db.Goals.Remove(goal);
    }
}
