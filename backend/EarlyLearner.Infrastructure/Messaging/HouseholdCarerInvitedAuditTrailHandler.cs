using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Infrastructure.Persistence;
using EarlyLearner.Infrastructure.Persistence.Entities;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class HouseholdCarerInvitedAuditTrailHandler(DatabaseContext db) : IDomainEventHandler
{
    public Type EventType => typeof(HouseholdCarerInvited);

    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent is not HouseholdCarerInvited carerInvited) {
            throw new InvalidOperationException($"{nameof(HouseholdCarerInvitedAuditTrailHandler)} cannot handle {domainEvent.GetType().Name}.");
        }

        db.AuditTrailEntries.Add(new AuditTrailReadModel {
            Id = Guid.NewGuid(),
            HouseholdId = carerInvited.HouseholdId.Value,
            Action = "HouseholdCarerInvited",
            Summary = "Carer invited",
            Details = $"{carerInvited.Email} was invited to join {carerInvited.HouseholdName}.",
            ActionedAt = carerInvited.OccurredAt,
            RecordedAt = DateTimeOffset.UtcNow
        });

        await Task.CompletedTask;
    }
}
