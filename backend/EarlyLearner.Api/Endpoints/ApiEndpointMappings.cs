using EarlyLearner.Api.Models;

namespace EarlyLearner.Api.Endpoints;

public static class ApiEndpointMappings
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(pattern: "/health", handler: () => Results.Ok(new ApiResponse(StatusCodes.Status200OK, "Healthy")))
            .WithName(endpointName: "HealthCheck")
            .WithTags(tags: "Health")
            .Produces<ApiResponse>(StatusCodes.Status200OK);

        return endpoints;
    }
}
