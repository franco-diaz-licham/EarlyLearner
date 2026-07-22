using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Tests.Fakes;
using EarlyLearner.Worker.Configuration.Options;
using EarlyLearner.Worker.Messaging;
using EarlyLearner.Worker.Messaging.Consumers;
using EarlyLearner.Worker.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace EarlyLearner.Shared.Tests.Fixtures;

public abstract class WorkerConsumerUnitTestFixture : IAsyncDisposable
{
    protected Mock<IEmailSender> _emailSender = default!;
    protected InMemoryDocumentStore _documentStore = default!;
    protected AuditDbContext _db = default!;
    protected HouseholdInvitationEmailRequestedConsumer _householdInvitationEmailRequestedConsumer = default!;
    protected AuditTrailEntryRecordedConsumer _auditTrailEntryRecordedConsumer = default!;

    [SetUp]
    public void SetUp()
    {
        _emailSender = new Mock<IEmailSender>(MockBehavior.Strict);
        _documentStore = new InMemoryDocumentStore();
        _db = new AuditDbContext(new DbContextOptionsBuilder<AuditDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        _householdInvitationEmailRequestedConsumer = new HouseholdInvitationEmailRequestedConsumer(
            _emailSender.Object,
            _documentStore,
            Microsoft.Extensions.Options.Options.Create(new EarlyLearnerOptions { Url = new Uri("https://earlylearner.test") }));
        _auditTrailEntryRecordedConsumer = new AuditTrailEntryRecordedConsumer(_db);
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await _db.DisposeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _db.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    protected static Mock<ConsumeContext<TMessage>> CreateContext<TMessage>(TMessage message) where TMessage : class
    {
        var context = new Mock<ConsumeContext<TMessage>>(MockBehavior.Strict);
        context
            .SetupGet(consumeContext => consumeContext.Message)
            .Returns(message);
        context
            .SetupGet(consumeContext => consumeContext.CancellationToken)
            .Returns(CancellationToken.None);

        return context;
    }
}