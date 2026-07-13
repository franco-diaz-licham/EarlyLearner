using EarlyLearner.Application.UseCases.LearningContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext.Entities;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Persistence.Repositories;

public sealed class DailyLogRepository(DatabaseContext db) : IDailyLogQueryRepository, IDailyLogCommandRepository
{
    public async Task<List<DailyLogResponse>> ListAsync(HouseholdId householdId, CancellationToken cancellationToken)
    {
        return await db.DailyLogs
            .AsNoTracking()
            .AsSplitQuery()
            .Where(log => log.HouseholdId == householdId)
            .OrderByDescending(log => log.LogDate)
            .Select(log => new DailyLogResponse(
                DailyLogId: log.Id.Value,
                HouseholdId: log.HouseholdId.Value,
                ChildId: log.ChildId.Value,
                LogDate: log.LogDate,
                LearningMomentCount: log.LearningMoments.Count,
                LearningMoments: log.LearningMoments
                    .OrderByDescending(moment => moment.CreatedOn)
                    .Select(moment => new LearningMomentResponse(
                        moment.Id.Value,
                        moment.Kind,
                        moment.Title,
                        moment.Notes,
                        moment.LearningOutcomes.Select(outcome => outcome.Id.Value).ToList()))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ChildExistsAsync(HouseholdId householdId, ChildId childId, CancellationToken cancellationToken)
    {
        return db.Children.AnyAsync(child => child.Id == childId && child.HouseholdId == householdId, cancellationToken);
    }

    public Task<List<LearningOutcome>> GetLearningOutcomesAsync(IReadOnlyList<LearningOutcomeId> learningOutcomeIds, CancellationToken cancellationToken)
    {
        return db.LearningOutcomes
            .Where(outcome => learningOutcomeIds.Contains(outcome.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<DailyLog?> GetByChildAndDateAsync(HouseholdId householdId, ChildId childId, DateOnly logDate, CancellationToken cancellationToken)
    {
        return db.DailyLogs
            .Include(log => log.LearningMoments)
            .ThenInclude(moment => moment.LearningOutcomes)
            .AsSplitQuery()
            .SingleOrDefaultAsync(log => log.HouseholdId == householdId && log.ChildId == childId && log.LogDate == logDate, cancellationToken);
    }

    public Task<DailyLog?> GetAsync(DailyLogId dailyLogId, CancellationToken cancellationToken)
    {
        return db.DailyLogs
            .Include(log => log.LearningMoments)
            .ThenInclude(moment => moment.LearningOutcomes)
            .AsSplitQuery()
            .SingleOrDefaultAsync(item => item.Id == dailyLogId, cancellationToken);
    }

    public async Task<DailyLogResponse?> GetResponseAsync(DailyLogId dailyLogId, CancellationToken cancellationToken)
    {
        return await db.DailyLogs
            .AsNoTracking()
            .AsSplitQuery()
            .Where(item => item.Id == dailyLogId)
            .Select(item => new DailyLogResponse(
                DailyLogId: item.Id.Value,
                HouseholdId: item.HouseholdId.Value,
                ChildId: item.ChildId.Value,
                LogDate: item.LogDate,
                LearningMomentCount: item.LearningMoments.Count,
                LearningMoments: item.LearningMoments
                    .OrderByDescending(moment => moment.CreatedOn)
                    .Select(moment => new LearningMomentResponse(
                        moment.Id.Value,
                        moment.Kind,
                        moment.Title,
                        moment.Notes,
                        moment.LearningOutcomes.Select(outcome => outcome.Id.Value).ToList()))
                    .ToList()))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public void Add(DailyLog dailyLog)
    {
        db.DailyLogs.Add(dailyLog);
    }

    public void Remove(DailyLog dailyLog)
    {
        db.DailyLogs.Remove(dailyLog);
    }

    public void RemoveLearningMoment(LearningMoment learningMoment)
    {
        db.LearningMoments.Remove(learningMoment);
    }
}
