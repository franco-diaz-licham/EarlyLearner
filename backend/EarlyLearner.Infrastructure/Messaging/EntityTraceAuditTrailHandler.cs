using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Shared.AuditContext;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class EntityTraceAuditTrailHandler(IIntegrationEventPublisher integrationEventPublisher) : IDomainEventHandler
{
    public Type EventType => typeof(EntityTraceRecorded);

    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent is not EntityTraceRecorded) throw new InvalidOperationException($"{nameof(EntityTraceAuditTrailHandler)} cannot handle {domainEvent.GetType().Name}.");
        var entityTrace = (EntityTraceRecorded)domainEvent;

        await integrationEventPublisher.PublishAsync(new AuditTrailEntryRecorded(
            Id: Guid.NewGuid(),
            HouseholdId: entityTrace.HouseholdId,
            Action: entityTrace.Action,
            Summary: entityTrace.Summary,
            Details: entityTrace.Details,
            ActionedAt: entityTrace.OccurredAt,
            OccurredAt: entityTrace.OccurredAt), cancellationToken);
    }
}
