using EarlyLearner.Shared.Tests.Fixtures;
using EarlyLearner.Worker.Persistence;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace EarlyLearner.Worker.Tests.Messaging.Consumers;

[TestFixture]
public sealed class AuditTrailEntryRecordedConsumerTests : WorkerConsumerFixture
{
    [Test]
    public async Task Consume_Should_RecordAuditTrailEntry_WhenEntryDoesNotExist()
    {
        // Arrange
        var message = TestData.CreateAuditTrailEntryRecordedEvent();
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
        var message = TestData.CreateAuditTrailEntryRecordedEvent();
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
}
