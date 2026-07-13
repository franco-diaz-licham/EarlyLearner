using EarlyLearner.Shared.Utilities;
using EarlyLearner.Domain.LearningContext;

namespace EarlyLearner.Application.UseCases.Dashboard;

public sealed record GetHomeDashboardQuery(DateOnly Today);

public sealed record GetHomeDashboardResponse(
    IReadOnlyList<HomeDashboardChildResponse> Children,
    IReadOnlyList<HomeDashboardMetricResponse> Metrics,
    IReadOnlyList<HomeDashboardRecentActivityResponse> RecentActivities,
    HomeDashboardTodaySummaryResponse Today,
    HomeDashboardOutcomeCoverageResponse OutcomeCoverage,
    IReadOnlyList<HomeDashboardRecentMomentResponse> RecentMoments);

public sealed record HomeDashboardChildResponse(
    Guid ChildId,
    string GivenName,
    DateOnly DateOfBirth);

public sealed record HomeDashboardMetricResponse(
    string Label,
    int Value,
    string Detail);

public sealed record HomeDashboardRecentActivityResponse(
    Guid DailyLogId,
    Guid ChildId,
    DateOnly LogDate,
    int LearningMomentCount);

public sealed record HomeDashboardTodaySummaryResponse(
    int DailyLogCount,
    int LearningMomentCount,
    int ChildrenObservedCount);

public sealed record HomeDashboardOutcomeCoverageResponse(
    int ActiveOutcomeCount,
    int TouchedThisWeekCount,
    int UntouchedActiveOutcomeCount);

public sealed record HomeDashboardRecentMomentResponse(
    Guid DailyLogId,
    Guid LearningMomentId,
    Guid ChildId,
    string ChildName,
    DateOnly LogDate,
    LearningMomentKindEnum Kind,
    string Title,
    string Notes,
    IReadOnlyList<string> OutcomeNames);

public interface IGetHomeDashboardQueryHandler
{
    Task<Result<GetHomeDashboardResponse>> HandleAsync(GetHomeDashboardQuery query, CancellationToken cancellationToken);
}
