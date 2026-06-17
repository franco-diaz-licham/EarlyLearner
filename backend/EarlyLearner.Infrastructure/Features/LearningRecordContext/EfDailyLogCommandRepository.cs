using EarlyLearner.Application.Features.LearningRecordContext;
using EarlyLearner.Domain.LearningRecordContext.Entities;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.LearningRecordContext;

public sealed class EfDailyLogCommandRepository(DatabaseContext db) : IDailyLogCommandRepository
{
    public Task<bool> ChildExistsAsync(Guid householdId, Guid childId, CancellationToken cancellationToken)
    {
        return db.Children.AnyAsync(child => child.Id.Value == childId && child.HouseholdId.Value == householdId, cancellationToken);
    }

    public Task<DailyLog?> GetAsync(Guid dailyLogId, CancellationToken cancellationToken)
    {
        return db.DailyLogs.SingleOrDefaultAsync(item => item.Id.Value == dailyLogId, cancellationToken);
    }

    public async Task<DailyLogResponse?> GetResponseAsync(Guid dailyLogId, CancellationToken cancellationToken)
    {
        return await db.DailyLogs
            .AsNoTracking()
            .Where(item => item.Id.Value == dailyLogId)
            .Select(item => new DailyLogResponse(
                DailyLogId: item.Id.Value,
                HouseholdId: item.HouseholdId.Value,
                ChildId: item.ChildId.Value,
                LogDate: item.LogDate,
                CompletedActivityCount: item.CompletedActivities.Count,
                ReadingEntryCount: item.ReadingEntries.Count,
                RoutineEntryCount: item.RoutineEntries.Count))
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
