using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Infrastructure.Persistence;
using EarlyLearner.Infrastructure.Persistence.Entities;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class EntityTraceAuditTrailHandler(DatabaseContext db) : IDomainEventHandler
{
    public Type EventType => typeof(EntityTraceRecorded);

    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent is not EntityTraceRecorded entityTrace)
        {
            throw new InvalidOperationException($"{nameof(EntityTraceAuditTrailHandler)} cannot handle {domainEvent.GetType().Name}.");
        }

        db.AuditTrailEntries.Add(new AuditTrailEntry
        {
            Id = Guid.NewGuid(),
            HouseholdId = entityTrace.HouseholdId,
            Action = entityTrace.Action,
            Summary = entityTrace.Summary,
            Details = entityTrace.Details,
            ActionedAt = entityTrace.OccurredAt,
            RecordedAt = DateTimeOffset.UtcNow
        });

        await Task.CompletedTask;
    }
}
