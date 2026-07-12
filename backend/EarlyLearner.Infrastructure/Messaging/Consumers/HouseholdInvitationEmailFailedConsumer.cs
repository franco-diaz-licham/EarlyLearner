using EarlyLearner.Application.Ports;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EarlyLearner.Infrastructure.Messaging.Consumers;

public sealed class HouseholdInvitationEmailFailedConsumer(
    IDocumentStore documentStore,
    INotificationPublisher notificationPublisher,
    ILogger<HouseholdInvitationEmailFailedConsumer> logger) : IConsumer<HouseholdInvitationEmailFailedEvent>
{
    public async Task Consume(ConsumeContext<HouseholdInvitationEmailFailedEvent> context)
    {
        var message = context.Message;
        var notification = await GetNotificationAsync(message.HouseholdId, message.InvitationId, context.CancellationToken);
        if (notification is null) {
            logger.LogWarning(
                "Notification document was not found for household {HouseholdId} and invitation {InvitationId}.",
                message.HouseholdId,
                message.InvitationId);

            return;
        }

        logger.LogInformation(
            "Publishing invitation email failed notification for household {HouseholdId} and invitation {InvitationId}.",
            message.HouseholdId,
            message.InvitationId);

        await notificationPublisher.PublishAsync(new NotificationResponse(
            Id: notification.InvitationId,
            HouseholdId: notification.HouseholdId,
            Type: notification.Type,
            Title: notification.Title,
            Message: notification.Message,
            OccurredAt: notification.OccurredAt), context.CancellationToken);
    }

    private Task<NotificationDocument?> GetNotificationAsync(Guid householdId, Guid invitationId, CancellationToken cancellationToken)
    {
        return documentStore.GetAsync<NotificationDocument>(
            NotificationDocument.ContainerName,
            NotificationDocument.BuildId(invitationId),
            NotificationDocument.BuildPartitionKey(householdId),
            cancellationToken);
    }
}
