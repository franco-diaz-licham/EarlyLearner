using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.UseCases.Dashboard;

namespace EarlyLearner.Api.Endpoints;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/dashboard")
            .WithTags("Dashboard");

        group.MapGet("/home", GetHomeDashboardAsync)
            .WithName(nameof(GetHomeDashboardAsync));

        return endpoints;
    }

    private static async Task<IResult> GetHomeDashboardAsync(
        DateOnly? today,
        IGetHomeDashboardQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetHomeDashboardQuery(today ?? DateOnly.FromDateTime(DateTime.UtcNow));
        var dashboard = await handler.HandleAsync(query, cancellationToken);
        return dashboard.ToApiResult();
    }
}
