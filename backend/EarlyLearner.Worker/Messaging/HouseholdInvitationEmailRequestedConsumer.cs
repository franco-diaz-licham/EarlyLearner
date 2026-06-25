using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Ports;
using MassTransit;

namespace EarlyLearner.Worker.Messaging;

public sealed class HouseholdInvitationEmailRequestedConsumerDefinition : ConsumerDefinition<HouseholdInvitationEmailRequestedConsumer>
{
    public HouseholdInvitationEmailRequestedConsumerDefinition()
    {
        EndpointName = MessagingConstants.HouseholdInvitationEmailRequestedEndpoint;
    }
}

public sealed class HouseholdInvitationEmailRequestedConsumer(IEmailSender emailSender) : IConsumer<HouseholdInvitationEmailRequested>
{
    public async Task Consume(ConsumeContext<HouseholdInvitationEmailRequested> context)
    {
        var message = context.Message;

        try {
            await emailSender.SendAsync(CreateEmail(message), context.CancellationToken);

            await context.Publish(new HouseholdInvitationEmailSent(
                Id: Guid.NewGuid(),
                InvitationId: message.InvitationId,
                Email: message.Email,
                SentAt: DateTimeOffset.UtcNow,
                OccurredAt: DateTimeOffset.UtcNow), context.CancellationToken);
        } catch (Exception exception) {
            await context.Publish(new HouseholdInvitationEmailFailed(
                Id: Guid.NewGuid(),
                InvitationId: message.InvitationId,
                Email: message.Email,
                Reason: exception.Message,
                FailedAt: DateTimeOffset.UtcNow,
                OccurredAt: DateTimeOffset.UtcNow), context.CancellationToken);
        }
    }

    private static EmailMessage CreateEmail(HouseholdInvitationEmailRequested message)
    {
        var greetingName = string.IsNullOrWhiteSpace(message.FirstName) ? "there" : message.FirstName.Trim();
        var subject = $"You're invited to join {message.HouseholdName}";
        var body = $"Hi {greetingName}, you have been invited to join {message.HouseholdName} on EarlyLearner. This invitation expires on {message.ExpiresAt:dd MMM yyyy}.";

        return new EmailMessage(message.Email, subject, body);
    }
}
