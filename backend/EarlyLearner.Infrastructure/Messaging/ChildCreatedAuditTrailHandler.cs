using EarlyLearner.Application.Features.AuditContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class ChildCreatedAuditTrailHandler(IIntegrationEventPublisher integrationEventPublisher) : IDomainEventHandler
{
    public Type EventType => typeof(ChildCreated);

    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent is not ChildCreated childCreated) {
            throw new InvalidOperationException($"{nameof(ChildCreatedAuditTrailHandler)} cannot handle {domainEvent.GetType().Name}.");
        }

        await integrationEventPublisher.PublishAsync(new AuditTrailRecordRequested(
            Id: Guid.NewGuid(),
            HouseholdId: childCreated.HouseholdId.Value,
            Action: "ChildCreated",
            Summary: "Child profile created",
            Details: $"Child profile {childCreated.ChildId.Value} was created.",
            ActionedAt: childCreated.OccurredAt,
            OccurredAt: childCreated.OccurredAt), cancellationToken);
    }
}
