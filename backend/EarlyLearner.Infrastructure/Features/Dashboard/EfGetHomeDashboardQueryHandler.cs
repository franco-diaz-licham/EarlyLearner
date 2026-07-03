using EarlyLearner.Application.Features.Dashboard;
using EarlyLearner.Application.Ports;
using EarlyLearner.Infrastructure.Persistence;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.Dashboard;

public sealed class EfGetHomeDashboardQueryHandler(DatabaseContext db, ICurrentUser currentUser) : IGetHomeDashboardQueryHandler
{
    public async Task<Result<GetHomeDashboardResponse>> HandleAsync(GetHomeDashboardQuery query, CancellationToken cancellationToken)
    {
        var householdId = currentUser.HouseholdId;

        var children = await db.Children
            .AsNoTracking()
            .Where(child => child.HouseholdId == householdId && !child.IsArchived)
            .OrderBy(child => child.FirstName)
            .Select(child => new HomeDashboardChildResponse(
                ChildId: child.Id.Value,
                GivenName: child.FirstName,
                DateOfBirth: child.DateOfBirth))
            .ToListAsync(cancellationToken);

        var readinessProfileCount = await db.ReadinessProfiles
            .AsNoTracking()
            .CountAsync(profile => profile.HouseholdId == householdId, cancellationToken);

        var weeklyRecordCount = await db.DailyLogs
            .AsNoTracking()
            .CountAsync(
                log => log.HouseholdId == householdId && log.LogDate >= query.Today.AddDays(-6),
                cancellationToken);

        var weeklyCompletedActivityCount = await db.CompletedActivities
            .AsNoTracking()
            .CountAsync(
                activity =>
                    activity.DailyLog.HouseholdId == householdId &&
                    activity.DailyLog.LogDate >= query.Today.AddDays(-6),
                cancellationToken);

        var recentActivities = await db.DailyLogs
            .AsNoTracking()
            .Where(log => log.HouseholdId == householdId)
            .OrderByDescending(log => log.LogDate)
            .Select(log => new HomeDashboardRecentActivityResponse(
                DailyLogId: log.Id.Value,
                ChildId: log.ChildId.Value,
                LogDate: log.LogDate,
                CompletedActivityCount: log.CompletedActivities.Count,
                ReadingEntryCount: log.ReadingEntries.Count,
                RoutineEntryCount: log.RoutineEntries.Count))
            .Take(5)
            .ToListAsync(cancellationToken);

        var metrics = new List<HomeDashboardMetricResponse> {
            new(Label: "Active children", Value: children.Count, Detail: "Children currently visible in this household"),
            new(Label: "Readiness profiles", Value: readinessProfileCount, Detail: "Children with readiness progress tracking"),
            new(Label: "Activities this week", Value: weeklyCompletedActivityCount, Detail: "Completed learning activities captured in the last seven days"),
            new(Label: "Records this week", Value: weeklyRecordCount, Detail: "Daily logs captured in the last seven days")
        };

        return Result<GetHomeDashboardResponse>.Success(
            new GetHomeDashboardResponse(
                Children: children,
                Metrics: metrics,
                RecentActivities: recentActivities),
            ResultTypeEnum.Success);
    }
}
