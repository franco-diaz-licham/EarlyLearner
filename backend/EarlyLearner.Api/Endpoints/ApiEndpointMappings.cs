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
            .Produces<ApiResponse>(StatusCodes.Status200OK);

        var api = endpoints.MapGroup($"/{ApiRouteOptions.VersionedApiPrefix}");

        api.MapCoreEndpoints();
        api.MapDashboardEndpoints();
        api.MapIdentityEndpoints();
        api.MapLearningRecordEndpoints();
        api.MapPlanningEndpoints();
        api.MapReadinessEndpoints();

        return endpoints;
    }
}
