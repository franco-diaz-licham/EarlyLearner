using EarlyLearner.Application.Ports;
using Microsoft.AspNetCore.SignalR;

namespace EarlyLearner.Api.Notifications;

public sealed class NotificationHubPublisher(IHubContext<NotificationHub> hubContext, ILogger<NotificationHubPublisher> logger) : INotificationPublisher
{
    public async ValueTask PublishAsync(NotificationResponse notification, CancellationToken cancellationToken = default)
    {
        var groupName = NotificationHub.BuildGroupName(notification.HouseholdId, notification.Id);
        logger.LogInformation("Publishing notification {NotificationId} to SignalR group {GroupName}.", notification.Id, groupName);
        await hubContext.Clients.Group(groupName).SendAsync(NotificationHub.NotificationReceivedMethod, notification, cancellationToken);
    }
}
