namespace EarlyLearner.Application.Features.Dashboard;

public sealed record GetHomeDashboardQuery(Guid HouseholdId, DateOnly Today);

public sealed record GetHomeDashboardResult(
    IReadOnlyList<HomeDashboardChildResult> Children,
    IReadOnlyList<HomeDashboardMetricResult> Metrics,
    IReadOnlyList<HomeDashboardPlannedSessionResult> UpcomingSessions,
    IReadOnlyList<HomeDashboardRecentActivityResult> RecentActivities);

public sealed record HomeDashboardChildResult(
    Guid ChildId,
    string GivenName,
    DateOnly DateOfBirth);

public sealed record HomeDashboardMetricResult(
    string Label,
    int Value,
    string Detail);

public sealed record HomeDashboardPlannedSessionResult(
    Guid SessionId,
    Guid LearningPlanId,
    DateOnly PlannedDate,
    string Title,
    string Status);

public sealed record HomeDashboardRecentActivityResult(
    Guid DailyLogId,
    Guid ChildId,
    DateOnly LogDate,
    int CompletedActivityCount,
    int ReadingEntryCount,
    int RoutineEntryCount);

public interface IGetHomeDashboardQueryHandler
{
    Task<GetHomeDashboardResult> HandleAsync(GetHomeDashboardQuery query, CancellationToken cancellationToken);
}
