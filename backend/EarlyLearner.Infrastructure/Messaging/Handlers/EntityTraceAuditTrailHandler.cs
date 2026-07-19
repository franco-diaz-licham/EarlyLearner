using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Shared.Messaging;

namespace EarlyLearner.Infrastructure.Messaging.Handlers;

public sealed class EntityTraceAuditTrailHandler(IIntegrationEventPublisher integrationEventPublisher) : IDomainEventHandler
{
    public Type EventType => typeof(EntityTraceRecordedEvent);

    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent is not EntityTraceRecordedEvent) throw new InvalidOperationException($"{nameof(EntityTraceAuditTrailHandler)} cannot handle {domainEvent.GetType().Name}.");
        var entityTrace = (EntityTraceRecordedEvent)domainEvent;

        await integrationEventPublisher.PublishAsync(new AuditTrailEntryRecordedEvent(
            Id: Guid.NewGuid(),
            HouseholdId: entityTrace.HouseholdId,
            Action: entityTrace.Action,
            Summary: entityTrace.Summary,
            Details: entityTrace.Details,
            ActionedAt: entityTrace.OccurredAt,
            OccurredAt: entityTrace.OccurredAt), cancellationToken);
    }
}
