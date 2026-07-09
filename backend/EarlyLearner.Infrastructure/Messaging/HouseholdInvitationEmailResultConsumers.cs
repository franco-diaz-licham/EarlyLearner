using EarlyLearner.Application.Features.Notifications;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using MassTransit;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class HouseholdInvitationEmailSentConsumer(IDocumentStore documentStore, INotificationPublisher notificationPublisher) : IConsumer<HouseholdInvitationEmailSentEvent>
{
    public async Task Consume(ConsumeContext<HouseholdInvitationEmailSentEvent> context)
    {
        var message = context.Message;
        var notification = await GetNotificationAsync(message.HouseholdId, message.InvitationId, context.CancellationToken);
        if (notification is null) return;

        await notificationPublisher.PublishAsync(new NotificationDto(
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
