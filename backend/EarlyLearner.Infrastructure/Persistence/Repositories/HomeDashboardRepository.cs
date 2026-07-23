using EarlyLearner.Application.UseCases.Dashboard;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Infrastructure.Persistence;
using EarlyLearner.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Persistence.Repositories;

public sealed class HomeDashboardRepository(DatabaseContext db, ICurrentUser currentUser) : IHomeDashboardRepository
{
    public async Task<Result<GetHomeDashboardResponse>> GetAsync(GetHomeDashboardQuery query, CancellationToken cancellationToken)
    {
        var householdId = currentUser.HouseholdId;
        var weekStart = query.Today.AddDays(-6);

        var children = await db.Children
            .AsNoTracking()
            .Where(child => child.HouseholdId == householdId && !child.IsArchived)
            .OrderBy(child => child.FirstName)
            .Select(child => new HomeDashboardChildResponse(
                ChildId: child.Id.Value,
                GivenName: child.FirstName,
                DateOfBirth: child.DateOfBirth,
                AvatarStoredFileId: child.AvatarStoredFileId.HasValue ? child.AvatarStoredFileId.Value.Value : null))
            .ToListAsync(cancellationToken);

        var activeOutcomeCount = await db.LearningOutcomes
            .AsNoTracking()
            .CountAsync(outcome => outcome.HouseholdId == householdId && outcome.Status == LearningOutcomeStatusEnum.Active, cancellationToken);

        var weeklyRecordCount = await db.DailyLogs
            .AsNoTracking()
            .CountAsync(
                log => log.HouseholdId == householdId && log.LogDate >= weekStart,
                cancellationToken);

        var weeklyLearningMomentCount = await db.LearningMoments
            .AsNoTracking()
            .CountAsync(
                moment =>
                    moment.DailyLog.HouseholdId == householdId &&
                    moment.DailyLog.LogDate >= weekStart,
                cancellationToken);

        var todayDailyLogCount = await db.DailyLogs
            .AsNoTracking()
            .CountAsync(log => log.HouseholdId == householdId && log.LogDate == query.Today, cancellationToken);

        var todayLearningMomentCount = await db.LearningMoments
            .AsNoTracking()
            .CountAsync(
                moment => moment.DailyLog.HouseholdId == householdId && moment.DailyLog.LogDate == query.Today,
                cancellationToken);

        var todayChildrenObservedCount = await db.DailyLogs
            .AsNoTracking()
            .Where(log => log.HouseholdId == householdId && log.LogDate == query.Today)
            .Select(log => log.ChildId)
            .Distinct()
            .CountAsync(cancellationToken);

        var touchedOutcomeNamesThisWeek = await db.LearningMoments
            .AsNoTracking()
            .Where(moment => moment.DailyLog.HouseholdId == householdId && moment.DailyLog.LogDate >= weekStart)
            .SelectMany(moment => moment.LearningOutcomes)
            .Where(outcome => outcome.Status == LearningOutcomeStatusEnum.Active)
            .Where(outcome => outcome.HouseholdId == householdId)
            .Select(outcome => outcome.Id)
            .Distinct()
            .CountAsync(cancellationToken);

        var recentMoments = await db.LearningMoments
            .AsNoTracking()
            .Where(moment => moment.DailyLog.HouseholdId == householdId)
            .OrderByDescending(moment => moment.DailyLog.LogDate)
            .ThenByDescending(moment => moment.CreatedOn)
            .Select(moment => new HomeDashboardRecentMomentResponse(
                moment.DailyLogId.Value,
                moment.Id.Value,
                moment.DailyLog.ChildId.Value,
                moment.DailyLog.Child.FirstName,
                moment.DailyLog.LogDate,
                moment.Kind,
                moment.Title,
                moment.Notes,
                moment.LearningOutcomes
                    .OrderBy(outcome => outcome.SortOrder)
                    .Select(outcome => outcome.Name)
                    .ToList()))
            .Take(5)
            .ToListAsync(cancellationToken);

        var today = new HomeDashboardTodaySummaryResponse(
            DailyLogCount: todayDailyLogCount,
            LearningMomentCount: todayLearningMomentCount,
            ChildrenObservedCount: todayChildrenObservedCount);

        var outcomeCoverage = new HomeDashboardOutcomeCoverageResponse(
            ActiveOutcomeCount: activeOutcomeCount,
            TouchedThisWeekCount: touchedOutcomeNamesThisWeek,
            UntouchedActiveOutcomeCount: Math.Max(0, activeOutcomeCount - touchedOutcomeNamesThisWeek));

        var pendingInvitationCount = await db.HouseholdInvitations
            .AsNoTracking()
            .CountAsync(
                invitation => invitation.HouseholdId == householdId && invitation.Status == HouseholdInvitationStatusEnum.Pending,
                cancellationToken);

        var recentActivities = await db.DailyLogs
            .AsNoTracking()
            .Where(log => log.HouseholdId == householdId)
            .OrderByDescending(log => log.LogDate)
            .Select(log => new HomeDashboardRecentActivityResponse(
                DailyLogId: log.Id.Value,
                ChildId: log.ChildId.Value,
                LogDate: log.LogDate,
                LearningMomentCount: log.LearningMoments.Count))
            .Take(5)
            .ToListAsync(cancellationToken);

        var metrics = new List<HomeDashboardMetricResponse> {
            new(Label: "Active children", Value: children.Count, Detail: "Children currently visible in this household"),
            new(Label: "Active outcomes", Value: activeOutcomeCount, Detail: "Outcome tags available for new learning moments"),
            new(Label: "Moments this week", Value: weeklyLearningMomentCount, Detail: "Learning moments captured in the last seven days"),
            new(Label: "Records this week", Value: weeklyRecordCount, Detail: "Daily logs captured in the last seven days"),
            new(Label: "Pending invitations", Value: pendingInvitationCount, Detail: "Carer invitations waiting for a response")
        };

        return Result<GetHomeDashboardResponse>.Success(
            new GetHomeDashboardResponse(
                Children: children,
                Metrics: metrics,
                RecentActivities: recentActivities,
                Today: today,
                OutcomeCoverage: outcomeCoverage,
                RecentMoments: recentMoments),
            ResultTypeEnum.Success);
    }
}
