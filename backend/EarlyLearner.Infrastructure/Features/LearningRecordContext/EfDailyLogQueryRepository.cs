using EarlyLearner.Application.Features.LearningRecordContext;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.LearningRecordContext;

public sealed class EfDailyLogQueryRepository(DatabaseContext db) : IDailyLogQueryRepository
{
    public async Task<List<DailyLogResponse>> ListAsync(Guid householdId, CancellationToken cancellationToken)
    {
        return await db.DailyLogs
            .AsNoTracking()
            .Where(log => log.HouseholdId.Value == householdId)
            .OrderByDescending(log => log.LogDate)
            .Select(log => new DailyLogResponse(
                DailyLogId: log.Id.Value,
                HouseholdId: log.HouseholdId.Value,
                ChildId: log.ChildId.Value,
                LogDate: log.LogDate,
                CompletedActivityCount: log.CompletedActivities.Count,
                ReadingEntryCount: log.ReadingEntries.Count,
                RoutineEntryCount: log.RoutineEntries.Count))
            .ToListAsync(cancellationToken);
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
}
