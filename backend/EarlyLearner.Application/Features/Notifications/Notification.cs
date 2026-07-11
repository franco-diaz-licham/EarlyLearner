namespace EarlyLearner.Application.Features.Notifications;

/// <summary>
/// Represents a notification payload delivered to connected API clients for a household invitation workflow.
/// This response is produced by application/infrastructure notification flows and sent over the realtime API boundary.
/// </summary>
public sealed record NotificationResponse(
    Guid Id,
    Guid HouseholdId,
    string Type,
    string Title,
    string Message,
    DateTimeOffset OccurredAt);

/// <summary>
/// Publishes application notification responses to connected clients through the API realtime boundary.
/// Implementations own the transport details, such as SignalR groups, while callers provide the notification payload.
/// </summary>
public interface INotificationPublisher
{
    ValueTask PublishAsync(NotificationResponse notification, CancellationToken cancellationToken = default);
}
