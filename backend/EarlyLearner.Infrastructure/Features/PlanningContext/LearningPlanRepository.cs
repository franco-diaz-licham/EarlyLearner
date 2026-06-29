using EarlyLearner.Application.Features.PlanningContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.PlanningContext.Entities;
using EarlyLearner.Domain.PlanningContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.PlanningContext;

public sealed class LearningPlanRepository(DatabaseContext db) : ILearningPlanQueryRepository, ILearningPlanCommandRepository
{
    public async Task<List<LearningPlanResponse>> ListAsync(HouseholdId householdId, CancellationToken cancellationToken)
    {
        return await db.LearningPlans
            .AsNoTracking()
            .Where(plan => plan.HouseholdId == householdId)
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

    public Task<bool> ChildExistsAsync(HouseholdId householdId, ChildId childId, CancellationToken cancellationToken)
    {
        return db.Children.AnyAsync(child => child.Id == childId && child.HouseholdId == householdId, cancellationToken);
    }

    public Task<LearningPlan?> GetAsync(LearningPlanId learningPlanId, CancellationToken cancellationToken)
    {
        return db.LearningPlans.SingleOrDefaultAsync(item => item.Id == learningPlanId, cancellationToken);
    }

    public async Task<LearningPlanResponse?> GetResponseAsync(LearningPlanId learningPlanId, CancellationToken cancellationToken)
    {
        return await db.LearningPlans
            .AsNoTracking()
            .Where(item => item.Id == learningPlanId)
            .Select(item => new LearningPlanResponse(
                LearningPlanId: item.Id.Value,
                HouseholdId: item.HouseholdId.Value,
                ChildId: item.ChildId.Value,
                StartDate: item.Period.StartDate,
                EndDate: item.Period.EndDate,
                Focus: item.Focus))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public void Add(LearningPlan learningPlan)
    {
        db.LearningPlans.Add(learningPlan);
    }

    public void Remove(LearningPlan learningPlan)
    {
        db.LearningPlans.Remove(learningPlan);
    }
}
