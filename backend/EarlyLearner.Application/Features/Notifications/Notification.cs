namespace EarlyLearner.Application.Features.Notifications;

public sealed record NotificationDto(
    Guid Id,
    Guid HouseholdId,
    string Type,
    string Title,
    string Message,
    DateTimeOffset OccurredAt);

public interface INotificationPublisher
{
    ValueTask PublishAsync(NotificationDto notification, CancellationToken cancellationToken = default);
}
