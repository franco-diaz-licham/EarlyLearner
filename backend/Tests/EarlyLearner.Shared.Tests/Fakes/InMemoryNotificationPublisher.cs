using EarlyLearner.Application.Ports;

namespace EarlyLearner.Shared.Tests.Fakes;

public sealed class InMemoryNotificationPublisher : INotificationPublisher
{
    private readonly List<NotificationResponse> _notifications = [];

    public IReadOnlyCollection<NotificationResponse> Notifications => _notifications;

    public ValueTask PublishAsync(NotificationResponse notification, CancellationToken cancellationToken = default)
    {
        _notifications.Add(notification);
        return ValueTask.CompletedTask;
    }
}
