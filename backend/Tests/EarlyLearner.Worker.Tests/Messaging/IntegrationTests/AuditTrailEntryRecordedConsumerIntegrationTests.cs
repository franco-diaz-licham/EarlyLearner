using EarlyLearner.Shared.Tests.Fixtures;
using EarlyLearner.Shared.Messaging;
using EarlyLearner.Worker.Persistence;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace EarlyLearner.Worker.Tests.Messaging.Consumers;

[TestFixture]
public sealed class AuditTrailEntryRecordedConsumerIntegrationTests : WorkerConsumerIntegrationTestFixture
{
    [Test]
    public async Task Consume_Should_RecordAuditTrailEntry_WhenEntryDoesNotExist()
    {
        // Arrange
        var message = TestData.CreateAuditTrailEntryRecordedEvent();
        var db = ResolveService<AuditDbContext>();
        var context = CreateContext(message);

        // Act
        await _auditTrailEntryRecordedConsumer.Consume(context.Object);

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

        // Act
        await _harness.Bus.Publish(message);
        await _harness.Bus.Publish(message);

        // Assert
        (await _harness.Consumed.Any<AuditTrailEntryRecordedEvent>()).ShouldBeTrue();

        var entries = await db.AuditTrailEntries.ToListAsync();
        entries.Count.ShouldBe(1);
        entries.Single().Id.ShouldBe(message.Id);
    }
}
