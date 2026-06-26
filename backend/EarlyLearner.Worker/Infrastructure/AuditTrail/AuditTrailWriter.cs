using EarlyLearner.Worker.Application.AuditTrail;
using EarlyLearner.Worker.Application.Ports;
using EarlyLearner.Worker.Infrastructure.Persistence;
using EarlyLearner.Worker.Infrastructure.Persistence.Entities;

namespace EarlyLearner.Worker.Infrastructure.AuditTrail;

public sealed class AuditTrailWriter(WorkerDbContext db) : IAuditTrailWriter
{
    public async Task AddAsync(AuditTrailEntryModel entry, CancellationToken cancellationToken)
    {
        db.AuditTrailEntries.Add(new AuditTrailEntry {
            Id = entry.Id,
            HouseholdId = entry.HouseholdId,
            Action = entry.Action,
            Summary = entry.Summary,
            Details = entry.Details,
            ActionedAt = entry.ActionedAt,
            RecordedAt = entry.RecordedAt
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}
