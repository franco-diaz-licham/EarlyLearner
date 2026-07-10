using EarlyLearner.Api.Models;
using EarlyLearner.Api.Configuration;
using EarlyLearner.Api.Notifications;
using EarlyLearner.Shared.Realtime;

namespace EarlyLearner.Api.Endpoints;

public static class ApiEndpointMappings
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthEndpoints();

        var api = endpoints.MapGroup($"/{ApiRouteOptions.VersionedApiPrefix}");

        api.MapRealtimeHubs();
        api.MapIdentityEndpoints();
        api.MapStoredFileEndpoints();
        api.MapDashboardEndpoints();
        api.MapHouseholdEndpoints();
        api.MapDailyLogEndpoints();
        api.MapReadinessOutcomeEndpoints();
        api.MapReadinessProfileEndpoints();

        return endpoints;
    }

    private static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(pattern: "/health", handler: () => Results.Ok(new ApiResponse(StatusCodes.Status200OK, "Healthy")))
            .WithName(endpointName: "HealthCheck")
            .WithTags(tags: "Health")
            .AllowAnonymous()
            .Produces<ApiResponse>(StatusCodes.Status200OK);

        endpoints.MapGet(pattern: "/health/ready", handler: () => Results.Ok(new ApiResponse(StatusCodes.Status200OK, "Ready")))
            .WithName(endpointName: "ReadinessCheck")
            .WithTags(tags: "Health")
            .AllowAnonymous()
            .Produces<ApiResponse>(StatusCodes.Status200OK);

        return endpoints;
    }

    private static IEndpointRouteBuilder MapRealtimeHubs(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<NotificationHub>(RealtimeHubRoutes.NotificationHub);

        return endpoints;
    }
}
