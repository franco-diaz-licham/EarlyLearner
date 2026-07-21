using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.Tests.Fixtures;
using EarlyLearner.Worker.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace EarlyLearner.Worker.Tests.Messaging.Consumers;

[TestFixture]
public sealed class AuditTrailEntryRecordedConsumerTests : WorkerConsumerFixture
{
    [Test]
    public async Task Consume_Should_RecordAuditTrailEntry_WhenEntryDoesNotExist()
    {
        // Arrange
        var message = CreateEvent();
        var db = ResolveService<AuditDbContext>();
        var sut = CreateAuditTrailEntryRecordedConsumer();
        var context = CreateContext(message);

        // Act
        await sut.Consume(context.Object);

        // Assert
        var entry = await db.AuditTrailEntries.SingleAsync();
        entry.Id.ShouldBe(message.Id);
        entry.HouseholdId.ShouldBe(message.HouseholdId);
        entry.Action.ShouldBe(message.Action);
        entry.Summary.ShouldBe(message.Summary);
        entry.Details.ShouldBe(message.Details);
        entry.ActionedAt.ShouldBe(message.ActionedAt);
        entry.RecordedAt.ShouldBeGreaterThan(DateTimeOffset.MinValue);
    }

    [Test]
    public async Task Consume_Should_NotCreateDuplicateAuditTrailEntry_WhenEntryAlreadyExists()
    {
        // Arrange
        var message = CreateEvent();
        var db = ResolveService<AuditDbContext>();
        var sut = CreateAuditTrailEntryRecordedConsumer();
        db.AuditTrailEntries.Add(new AuditTrailEntry {
            Id = message.Id,
            HouseholdId = message.HouseholdId,
            Action = message.Action,
            Summary = message.Summary,
            Details = message.Details,
            ActionedAt = message.ActionedAt,
            RecordedAt = DateTimeOffset.UtcNow.AddDays(-1)
        });
        await db.SaveChangesAsync();
        var context = CreateContext(message);

        // Act
        await sut.Consume(context.Object);

        // Assert
        var entries = await db.AuditTrailEntries.ToListAsync();
        entries.Count.ShouldBe(1);
        entries.Single().Id.ShouldBe(message.Id);
    }


    private static AuditTrailEntryRecordedEvent CreateEvent()
    {
        return new AuditTrailEntryRecordedEvent(
            Id: Guid.NewGuid(),
            HouseholdId: Guid.NewGuid(),
            Action: "HouseholdCarerInvited",
            Summary: "A household invitation was created.",
            Details: "carer@example.com",
            ActionedAt: DateTimeOffset.UtcNow.AddMinutes(-1),
            OccurredAt: DateTimeOffset.UtcNow);
    }
}


