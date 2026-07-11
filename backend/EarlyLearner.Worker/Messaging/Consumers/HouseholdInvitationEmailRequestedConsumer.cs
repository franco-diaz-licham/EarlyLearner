using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using EarlyLearner.Worker.Messaging;
using EarlyLearner.Worker.Options;
using MassTransit;
using Microsoft.Extensions.Options;

namespace EarlyLearner.Worker.Messaging.Consumers;

public sealed class HouseholdInvitationEmailRequestedConsumer(
    IEmailSender emailSender,
    IDocumentStore documentStore,
    IOptions<EarlyLearnerOptions> options) : IConsumer<HouseholdInvitationEmailRequestedEvent>
{
    private readonly EarlyLearnerOptions options = options.Value;

    public async Task Consume(ConsumeContext<HouseholdInvitationEmailRequestedEvent> context)
    {
        var message = context.Message;

        try {
            await emailSender.SendAsync(EmailBuilder.BuildHouseholdInvitationEmail(message, options.Url), context.CancellationToken);
            var emailSent = new HouseholdInvitationEmailSentEvent(
                Id: Guid.NewGuid(),
                HouseholdId: message.HouseholdId,
                InvitationId: message.InvitationId,
                Email: message.Email,
                SentAt: DateTimeOffset.UtcNow,
                OccurredAt: DateTimeOffset.UtcNow);

            await UpsertNotificationAsync(ToNotificationDocument(emailSent), context.CancellationToken);
            await context.Publish(emailSent, context.CancellationToken);
        } catch (Exception exception) {
            var emailFailed = new HouseholdInvitationEmailFailedEvent(
                Id: Guid.NewGuid(),
                HouseholdId: message.HouseholdId,
                InvitationId: message.InvitationId,
                Email: message.Email,
                Reason: exception.Message,
                FailedAt: DateTimeOffset.UtcNow,
                OccurredAt: DateTimeOffset.UtcNow);

            await UpsertNotificationAsync(ToNotificationDocument(emailFailed), context.CancellationToken);
            await context.Publish(emailFailed, context.CancellationToken);
        }
    }

    private static NotificationDocument ToNotificationDocument(HouseholdInvitationEmailSentEvent message) => new(
        Id: NotificationDocument.BuildId(message.InvitationId),
        HouseholdId: message.HouseholdId,
        InvitationId: message.InvitationId,
        Type: "householdInvitationEmailSent",
        Title: "Invitation email sent",
        Message: $"Invitation email was sent to {message.Email}.",
        Status: NotificationDeliveryStatus.Succeeded,
        OccurredAt: message.SentAt);

    private static NotificationDocument ToNotificationDocument(HouseholdInvitationEmailFailedEvent message) => new(
        Id: NotificationDocument.BuildId(message.InvitationId),
        HouseholdId: message.HouseholdId,
        InvitationId: message.InvitationId,
        Type: "householdInvitationEmailFailed",
        Title: "Invitation email failed",
        Message: $"Invitation email to {message.Email} failed: {message.Reason}",
        Status: NotificationDeliveryStatus.Failed,
        OccurredAt: message.FailedAt);

    private async Task UpsertNotificationAsync(NotificationDocument notification, CancellationToken cancellationToken)
    {
        await documentStore.UpsertAsync(
            NotificationDocument.ContainerName,
            notification,
            NotificationDocument.BuildPartitionKey(notification.HouseholdId),
            cancellationToken);
    }
}
