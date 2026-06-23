using EarlyLearner.Api.Models;
using EarlyLearner.Api.Configuration;

namespace EarlyLearner.Api.Endpoints;

public static class ApiEndpointMappings
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(pattern: "/health", handler: () => Results.Ok(new ApiResponse(StatusCodes.Status200OK, "Healthy")))
            .WithName(endpointName: "HealthCheck")
            .WithTags(tags: "Health")
            .AllowAnonymous()
            .Produces<ApiResponse>(StatusCodes.Status200OK);

        var api = endpoints.MapGroup($"/{ApiRouteOptions.VersionedApiPrefix}");

        api.MapIdentityEndpoints();
        api.MapStoredFileEndpoints();
        api.MapDashboardEndpoints();
        api.MapHouseholdEndpoints();
        api.MapDailyLogEndpoints();
        api.MapGoalEndpoints();
        api.MapLearningPlanEndpoints();
        api.MapReadinessOutcomeEndpoints();
        api.MapReadinessProfileEndpoints();

        return endpoints;
    }
}
