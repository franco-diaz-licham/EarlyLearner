using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.Dashboard;

public sealed record GetHomeDashboardQuery(DateOnly Today);

public sealed record GetHomeDashboardResponse(
    IReadOnlyList<HomeDashboardChildResponse> Children,
    IReadOnlyList<HomeDashboardMetricResponse> Metrics,
    IReadOnlyList<HomeDashboardRecentActivityResponse> RecentActivities);

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

public interface IGetHomeDashboardQueryHandler
{
    Task<Result<GetHomeDashboardResponse>> HandleAsync(GetHomeDashboardQuery query, CancellationToken cancellationToken);
}
