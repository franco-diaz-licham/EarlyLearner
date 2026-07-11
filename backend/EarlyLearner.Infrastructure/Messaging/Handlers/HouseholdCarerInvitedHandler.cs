using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Shared.Messaging;

namespace EarlyLearner.Infrastructure.Messaging.Handlers;

public sealed class HouseholdCarerInvitedHandler(IIntegrationEventPublisher integrationEventPublisher) : IDomainEventHandler
{
    public Type EventType => typeof(HouseholdCarerInvited);

    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent is not HouseholdCarerInvited carerInvited) {
            throw new InvalidOperationException($"{nameof(HouseholdCarerInvitedHandler)} cannot handle {domainEvent.GetType().Name}.");
        }

        await integrationEventPublisher.PublishAsync(new HouseholdInvitationEmailRequestedEvent(
            Id: Guid.NewGuid(),
            HouseholdId: carerInvited.HouseholdId.Value,
            InvitationId: carerInvited.InvitationId.Value,
            HouseholdName: carerInvited.HouseholdName,
            Email: carerInvited.Email,
            FirstName: carerInvited.FirstName,
            LastName: carerInvited.LastName,
            ExpiresAt: carerInvited.ExpiresAt,
            OccurredAt: carerInvited.OccurredAt), cancellationToken);
    }
}
