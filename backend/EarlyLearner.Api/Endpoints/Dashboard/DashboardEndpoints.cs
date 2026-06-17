using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.Dashboard;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

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
        Guid householdId,
        DateOnly? today,
        IGetHomeDashboardQueryHandler handler,
        CancellationToken cancellationToken)
    {
        if (householdId == Guid.Empty) {
            return Result<GetHomeDashboardResponse>
                .Fail("A household id is required to load the dashboard.", ResultTypeEnum.Invalid)
                .ToApiResult();
        }

        var query = new GetHomeDashboardQuery(householdId, today ?? DateOnly.FromDateTime(DateTime.UtcNow));
        var dashboard = await handler.HandleAsync(query, cancellationToken);
        return dashboard.ToApiResult();
    }
}
