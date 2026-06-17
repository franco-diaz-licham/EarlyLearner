using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace EarlyLearner.Api.Endpoints;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/dashboard")
            .WithTags("Dashboard");

        group.MapGet("/home", GetHomeDashboardAsync)
            .WithName("GetHomeDashboard")
            .Produces<GetHomeDashboardResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return endpoints;
    }

    private static async Task<IResult> GetHomeDashboardAsync(
        Guid householdId,
        DateOnly? today,
        IGetHomeDashboardQueryHandler handler,
        CancellationToken cancellationToken)
    {
        if (householdId == Guid.Empty) {
            return TypedResults.BadRequest(new ProblemDetails {
                Title = "Invalid household id",
                Detail = "A household id is required to load the dashboard.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var query = new GetHomeDashboardQuery(householdId, today ?? DateOnly.FromDateTime(DateTime.UtcNow));
        var dashboard = await handler.HandleAsync(query, cancellationToken);
        return dashboard.ToApiResult();
    }
}
