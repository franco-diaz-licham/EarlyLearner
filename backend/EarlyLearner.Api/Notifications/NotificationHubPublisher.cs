using EarlyLearner.Application.Features.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace EarlyLearner.Api.Notifications;

public sealed class NotificationHubPublisher(IHubContext<NotificationHub> hubContext) : INotificationPublisher
{
    public async ValueTask PublishAsync(NotificationDto notification, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients
            .Group(NotificationHub.BuildGroupName(notification.HouseholdId, notification.Id))
            .SendAsync(
                NotificationHub.NotificationReceivedMethod,
                notification,
                cancellationToken);
    }
}
