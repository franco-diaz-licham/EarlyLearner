using EarlyLearner.Shared.Messaging;
using EarlyLearner.Worker.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EarlyLearner.Worker.Messaging.Consumers;

public sealed class AuditTrailEntryRecordedConsumer(AuditDbContext db) : IConsumer<AuditTrailEntryRecordedEvent>
{
    public async Task Consume(ConsumeContext<AuditTrailEntryRecordedEvent> context)
    {
        var message = context.Message;

        var hasEntry = await db.AuditTrailEntries.AnyAsync(entry => entry.Id == message.Id, context.CancellationToken);
        if (hasEntry) return;

        db.AuditTrailEntries.Add(new AuditTrailEntry {
            Id = message.Id,
            HouseholdId = message.HouseholdId,
            Action = message.Action,
            Summary = message.Summary,
            Details = message.Details,
            ActionedAt = message.ActionedAt,
            RecordedAt = DateTimeOffset.UtcNow
        });

        try {
            await db.SaveChangesAsync(context.CancellationToken);
        } catch (DbUpdateException exception) when (IsDuplicateAuditTrailEntry(exception)) {
            db.ChangeTracker.Clear();
        }
    }

    private static bool IsDuplicateAuditTrailEntry(DbUpdateException exception)
    {
        return exception.GetBaseException() is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }
}
