using EarlyLearner.Application.Features.Dashboard;
using EarlyLearner.Domain.PlanningContext;
using EarlyLearner.Infrastructure.Persistence;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.Dashboard;

public sealed class EfGetHomeDashboardQueryHandler(DatabaseContext db) : IGetHomeDashboardQueryHandler
{
    public async Task<Result<GetHomeDashboardResponse>> HandleAsync(GetHomeDashboardQuery query, CancellationToken cancellationToken)
    {
        var children = await db.Children
            .AsNoTracking()
            .Where(child => child.HouseholdId.Value == query.HouseholdId && !child.IsArchived)
            .OrderBy(child => child.GivenName)
            .Select(child => new HomeDashboardChildResponse(
                ChildId: child.Id.Value,
                GivenName: child.GivenName,
                DateOfBirth: child.DateOfBirth))
            .ToListAsync(cancellationToken);

        var activeGoalCount = await db.Goals
            .AsNoTracking()
            .CountAsync(
                goal => goal.HouseholdId.Value == query.HouseholdId && goal.Status == GoalStatusEnum.Active,
                cancellationToken);

        var plannedSessionCount = await db.PlannedLearningSessions
            .AsNoTracking()
            .CountAsync(
                session =>
                    session.LearningPlan.HouseholdId.Value == query.HouseholdId &&
                    session.Status == SessionStatusEnum.Planned &&
                    session.PlannedDate >= query.Today,
                cancellationToken);

        var weeklyRecordCount = await db.DailyLogs
            .AsNoTracking()
            .CountAsync(
                log => log.HouseholdId.Value == query.HouseholdId && log.LogDate >= query.Today.AddDays(-6),
                cancellationToken);

        var upcomingSessions = await db.PlannedLearningSessions
            .AsNoTracking()
            .Where(session =>
                session.LearningPlan.HouseholdId.Value == query.HouseholdId &&
                session.Status == SessionStatusEnum.Planned &&
                session.PlannedDate >= query.Today)
            .OrderBy(session => session.PlannedDate)
            .ThenBy(session => session.Title)
            .Select(session => new HomeDashboardPlannedSessionResponse(
                SessionId: session.Id.Value,
                LearningPlanId: session.LearningPlanId.Value,
                PlannedDate: session.PlannedDate,
                Title: session.Title,
                Status: session.Status.ToString()))
            .Take(5)
            .ToListAsync(cancellationToken);

        var recentActivities = await db.DailyLogs
            .AsNoTracking()
            .Where(log => log.HouseholdId.Value == query.HouseholdId)
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
            new(Label: "Active goals", Value: activeGoalCount, Detail: "Goals available for planning and evidence"),
            new(Label: "Upcoming sessions", Value: plannedSessionCount, Detail: "Planned learning sessions from today onward"),
            new(Label: "Records this week", Value: weeklyRecordCount, Detail: "Daily logs captured in the last seven days")
        };

        return Result<GetHomeDashboardResponse>.Success(
            new GetHomeDashboardResponse(
                Children: children,
                Metrics: metrics,
                UpcomingSessions: upcomingSessions,
                RecentActivities: recentActivities),
            ResultTypeEnum.Success);
    }
}
