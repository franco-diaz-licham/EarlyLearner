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
            .Where(log => log.HouseholdId == householdId)
            .OrderByDescending(log => log.LogDate)
            .Select(log => new DailyLogResponse(
                DailyLogId: log.Id.Value,
                HouseholdId: log.HouseholdId.Value,
                ChildId: log.ChildId.Value,
                LogDate: log.LogDate,
                LearningMomentCount: log.LearningMoments.Count))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ChildExistsAsync(HouseholdId householdId, ChildId childId, CancellationToken cancellationToken)
    {
        return db.Children.AnyAsync(child => child.Id == childId && child.HouseholdId == householdId, cancellationToken);
    }

    public Task<DailyLog?> GetAsync(DailyLogId dailyLogId, CancellationToken cancellationToken)
    {
        return db.DailyLogs.SingleOrDefaultAsync(item => item.Id == dailyLogId, cancellationToken);
    }

    public async Task<DailyLogResponse?> GetResponseAsync(DailyLogId dailyLogId, CancellationToken cancellationToken)
    {
        return await db.DailyLogs
            .AsNoTracking()
            .Where(item => item.Id == dailyLogId)
            .Select(item => new DailyLogResponse(
                DailyLogId: item.Id.Value,
                HouseholdId: item.HouseholdId.Value,
                ChildId: item.ChildId.Value,
                LogDate: item.LogDate,
                LearningMomentCount: item.LearningMoments.Count))
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
}
