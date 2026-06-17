using EarlyLearner.Application.Features.PlanningContext;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.PlanningContext;

public sealed class EfLearningPlanQueryRepository(DatabaseContext db) : ILearningPlanQueryRepository
{
    public async Task<List<LearningPlanResponse>> ListAsync(Guid householdId, CancellationToken cancellationToken)
    {
        return await db.LearningPlans
            .AsNoTracking()
            .Where(plan => plan.HouseholdId.Value == householdId)
            .OrderByDescending(plan => plan.Period.StartDate)
            .Select(plan => new LearningPlanResponse(
                LearningPlanId: plan.Id.Value,
                HouseholdId: plan.HouseholdId.Value,
                ChildId: plan.ChildId.Value,
                StartDate: plan.Period.StartDate,
                EndDate: plan.Period.EndDate,
                Focus: plan.Focus))
            .ToListAsync(cancellationToken);
    }

    public async Task<LearningPlanResponse?> GetResponseAsync(Guid learningPlanId, CancellationToken cancellationToken)
    {
        return await db.LearningPlans
            .AsNoTracking()
            .Where(item => item.Id.Value == learningPlanId)
            .Select(item => new LearningPlanResponse(
                LearningPlanId: item.Id.Value,
                HouseholdId: item.HouseholdId.Value,
                ChildId: item.ChildId.Value,
                StartDate: item.Period.StartDate,
                EndDate: item.Period.EndDate,
                Focus: item.Focus))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
