using EarlyLearner.Shared.AuditContext;
using EarlyLearner.Worker.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Worker.Messaging;

public sealed class AuditTrailEntryRecordedConsumer(AuditDbContext db) : IConsumer<AuditTrailEntryRecorded>
{
    public async Task Consume(ConsumeContext<AuditTrailEntryRecorded> context)
    {
        var message = context.Message;

        if (await db.AuditTrailEntries.AnyAsync(entry => entry.Id == message.Id, context.CancellationToken)) return;
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
