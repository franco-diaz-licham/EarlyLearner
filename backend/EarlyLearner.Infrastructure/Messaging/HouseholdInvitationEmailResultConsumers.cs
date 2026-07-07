using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Features.Notifications;
using MassTransit;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class HouseholdInvitationEmailSentConsumer(INotificationPublisher notificationPublisher) : IConsumer<HouseholdInvitationEmailSent>
{
    public async Task Consume(ConsumeContext<HouseholdInvitationEmailSent> context)
    {
        var message = context.Message;

        await notificationPublisher.PublishAsync(new NotificationDto(
            Id: Guid.NewGuid(),
            HouseholdId: message.HouseholdId,
            Type: "householdInvitationEmailSent",
            Title: "Invitation email sent",
            Message: $"Invitation email was sent to {message.Email}.",
            OccurredAt: message.SentAt), context.CancellationToken);
    }
}

public sealed class HouseholdInvitationEmailFailedConsumer(INotificationPublisher notificationPublisher) : IConsumer<HouseholdInvitationEmailFailed>
{
    public async Task Consume(ConsumeContext<HouseholdInvitationEmailFailed> context)
    {
        var message = context.Message;

        await notificationPublisher.PublishAsync(new NotificationDto(
            Id: Guid.NewGuid(),
            HouseholdId: message.HouseholdId,
            Type: "householdInvitationEmailFailed",
            Title: "Invitation email failed",
            Message: $"Invitation email to {message.Email} failed: {message.Reason}",
            OccurredAt: message.FailedAt), context.CancellationToken);
    }
}
