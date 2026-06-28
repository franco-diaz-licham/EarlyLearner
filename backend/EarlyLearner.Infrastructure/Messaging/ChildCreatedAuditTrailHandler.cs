using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Infrastructure.Persistence;
using EarlyLearner.Infrastructure.Persistence.Entities;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class ChildCreatedAuditTrailHandler(DatabaseContext db) : IDomainEventHandler
{
    public Type EventType => typeof(ChildCreated);

    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent is not ChildCreated childCreated) {
            throw new InvalidOperationException($"{nameof(ChildCreatedAuditTrailHandler)} cannot handle {domainEvent.GetType().Name}.");
        }

        db.AuditTrailEntries.Add(new AuditTrailReadModel {
            Id = Guid.NewGuid(),
            HouseholdId = childCreated.HouseholdId.Value,
            Action = "ChildCreated",
            Summary = "Child profile created",
            Details = $"Child profile {childCreated.ChildId.Value} was created.",
            ActionedAt = childCreated.OccurredAt,
            RecordedAt = DateTimeOffset.UtcNow
        });

        await Task.CompletedTask;
    }
}
