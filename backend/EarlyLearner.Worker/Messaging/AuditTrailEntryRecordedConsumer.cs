using EarlyLearner.Shared.Messaging;
using EarlyLearner.Worker.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Worker.Messaging;

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

        await db.SaveChangesAsync(context.CancellationToken);
    }
}
