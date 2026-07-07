using System.Text.Json;
using EarlyLearner.Application.Features.Notifications;
using EarlyLearner.Application.Ports;

namespace EarlyLearner.Api.Endpoints.Notifications;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var notifications = endpoints.MapGroup("/notifications").WithTags("Notifications");

        notifications.MapGet("/stream", StreamNotifications)
            .WithName(nameof(StreamNotifications));

        return endpoints;
    }

    private static async Task StreamNotifications(
        HttpContext httpContext,
        ICurrentUser currentUser,
        INotificationStream notificationStream,
        CancellationToken cancellationToken)
    {
        httpContext.Response.Headers.CacheControl = "no-cache";
        httpContext.Response.Headers.Connection = "keep-alive";
        httpContext.Response.ContentType = "text/event-stream";

        try {
            await foreach (var notification in notificationStream.SubscribeAsync(currentUser.HouseholdId.Value, cancellationToken)) {
                var data = JsonSerializer.Serialize(notification, JsonSerializerOptions.Web);
                await httpContext.Response.WriteAsync($"event: notification\ndata: {data}\n\n", cancellationToken);
                await httpContext.Response.Body.FlushAsync(cancellationToken);
            }
        } catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
        }
    }
}
