using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Worker.Options;
using MassTransit;
using Microsoft.Extensions.Options;

namespace EarlyLearner.Worker.Messaging;

public sealed class HouseholdInvitationEmailRequestedConsumerDefinition : ConsumerDefinition<HouseholdInvitationEmailRequestedConsumer>
{
    public HouseholdInvitationEmailRequestedConsumerDefinition() => EndpointName = MessagingConstants.HouseholdInvitationEmailRequestedEndpoint;
}

public sealed class HouseholdInvitationEmailRequestedConsumer(IEmailSender emailSender, IOptions<EarlyLearnerOptions> options) : IConsumer<HouseholdInvitationEmailRequested>
{
    private readonly EarlyLearnerOptions options = options.Value;

    public async Task Consume(ConsumeContext<HouseholdInvitationEmailRequested> context)
    {
        var message = context.Message;

        try {
            await emailSender.SendAsync(EmailBuilder.BuildHouseholdInvitationEmail(message, options.Url), context.CancellationToken);
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
}
