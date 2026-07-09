using EarlyLearner.Application.Features.Notifications;
using EarlyLearner.Shared.Documents;
using EarlyLearner.Shared.IdentityContext;
using EarlyLearner.Shared.Notifications;
using MassTransit;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class HouseholdInvitationEmailFailedConsumer(IDocumentStore documentStore, INotificationPublisher notificationPublisher) : IConsumer<HouseholdInvitationEmailFailedEvent>
{
    public async Task Consume(ConsumeContext<HouseholdInvitationEmailFailedEvent> context)
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
