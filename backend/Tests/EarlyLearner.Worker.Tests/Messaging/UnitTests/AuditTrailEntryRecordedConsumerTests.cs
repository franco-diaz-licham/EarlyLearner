using EarlyLearner.Shared.Messaging;
using EarlyLearner.Worker.Messaging.Consumers;
using EarlyLearner.Worker.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace EarlyLearner.Worker.Tests.Messaging.Consumers;

[TestFixture]
public sealed class AuditTrailEntryRecordedConsumerTests
{
    private AuditDbContext _db = default!;
    private AuditTrailEntryRecordedConsumer _sut = default!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuditDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AuditDbContext(options);
        _sut = new AuditTrailEntryRecordedConsumer(_db);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _db.DisposeAsync();
    }

    [Test]
    public async Task Consume_Should_RecordAuditTrailEntry_WhenEntryDoesNotExist()
    {
        // Arrange
        var message = CreateEvent();
        var context = CreateContext(message);

        // Act
        await _sut.Consume(context.Object);

        // Assert
        var entry = await _db.AuditTrailEntries.SingleAsync();
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
        _db.AuditTrailEntries.Add(new AuditTrailEntry {
            Id = message.Id,
            HouseholdId = message.HouseholdId,
            Action = message.Action,
            Summary = message.Summary,
            Details = message.Details,
            ActionedAt = message.ActionedAt,
            RecordedAt = DateTimeOffset.UtcNow.AddDays(-1)
        });
        await _db.SaveChangesAsync();
        var context = CreateContext(message);

        // Act
        await _sut.Consume(context.Object);

        // Assert
        var entries = await _db.AuditTrailEntries.ToListAsync();
        entries.Count.ShouldBe(1);
        entries.Single().Id.ShouldBe(message.Id);
    }

    private static Mock<ConsumeContext<AuditTrailEntryRecordedEvent>> CreateContext(AuditTrailEntryRecordedEvent message)
    {
        var context = new Mock<ConsumeContext<AuditTrailEntryRecordedEvent>>(MockBehavior.Strict);
        context
            .SetupGet(consumeContext => consumeContext.Message)
            .Returns(message);
        context
            .SetupGet(consumeContext => consumeContext.CancellationToken)
            .Returns(CancellationToken.None);

        return context;
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