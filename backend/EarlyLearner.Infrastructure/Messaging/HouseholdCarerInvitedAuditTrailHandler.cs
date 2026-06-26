using EarlyLearner.Application.Features.AuditContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class HouseholdCarerInvitedAuditTrailHandler(IIntegrationEventPublisher integrationEventPublisher) : IDomainEventHandler
{
    public Type EventType => typeof(HouseholdCarerInvited);

    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent is not HouseholdCarerInvited carerInvited) {
            throw new InvalidOperationException($"{nameof(HouseholdCarerInvitedAuditTrailHandler)} cannot handle {domainEvent.GetType().Name}.");
        }

        await integrationEventPublisher.PublishAsync(new AuditTrailRecordRequested(
            Id: Guid.NewGuid(),
            HouseholdId: carerInvited.HouseholdId.Value,
            Action: "HouseholdCarerInvited",
            Summary: "Carer invited",
            Details: $"{carerInvited.Email} was invited to join {carerInvited.HouseholdName}.",
            ActionedAt: carerInvited.OccurredAt,
            OccurredAt: carerInvited.OccurredAt), cancellationToken);
    }
}
