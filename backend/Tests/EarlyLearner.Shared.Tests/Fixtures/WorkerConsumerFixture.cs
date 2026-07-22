using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Tests.Fakes;
using EarlyLearner.Worker.Configuration.Options;
using EarlyLearner.Worker.Messaging;
using EarlyLearner.Worker.Messaging.Consumers;
using EarlyLearner.Worker.Persistence;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EarlyLearner.Shared.Tests.Fixtures;

/// <summary>
/// Provides an isolated in-process worker host for consumer integration testing.
/// Creates a fresh host, scope, test doubles, and MassTransit test harness for each test.
/// </summary>
public abstract class WorkerConsumerFixture
{
    protected ITestHarness _harness = default!;
    protected Mock<IEmailSender> _emailSender = default!;
    protected InMemoryDocumentStore _documentStore = default!;
    protected HouseholdInvitationEmailRequestedConsumer _householdInvitationEmailRequestedConsumer = default!;
    protected AuditTrailEntryRecordedConsumer _auditTrailEntryRecordedConsumer = default!;

    private IHost? _host;
    private IServiceScope? _testServiceScope;

    [SetUp]
    public async Task StartHostAsync()
    {
        _host = ConfigureInProcessHost();
        await _host.StartAsync();

        _testServiceScope = _host.Services.CreateScope();
        _harness = ResolveService<ITestHarness>();
        await _harness.Start();

        _householdInvitationEmailRequestedConsumer = CreateHouseholdInvitationEmailRequestedConsumer();
        _auditTrailEntryRecordedConsumer = CreateAuditTrailEntryRecordedConsumer();
    }

    [TearDown]
    public async Task TearDownHostAsync()
    {
        if (_harness is not null) await _harness.Stop();
        if (_host is null) return;

        try {
            _testServiceScope?.Dispose();
            await _host.StopAsync();
        } finally {
            _host.Dispose();
        }
    }

    protected T ResolveService<T>() where T : notnull
    {
        if (_testServiceScope is null) throw new InvalidOperationException("Test service scope is not initialized.");
        return _testServiceScope.ServiceProvider.GetRequiredService<T>();
    }

    private IHost ConfigureInProcessHost()
    {
        var workerAssembly = typeof(WorkerProgram).Assembly;
        var workerAssemblyDirectory = Path.GetDirectoryName(workerAssembly.Location);
        if (string.IsNullOrWhiteSpace(workerAssemblyDirectory)) throw new InvalidOperationException("Could not determine worker assembly directory.");

        return new HostBuilder()
            .UseContentRoot(workerAssemblyDirectory)
            .ConfigureAppConfiguration(config => {
                config.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: true, reloadOnChange: false);
            })
            .UseDefaultServiceProvider(options => {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            })
            .ConfigureLogging(logging => {
                logging.AddSimpleConsole();
            })
            .ConfigureServices((_, services) => {
                ConfigureHostServices(services);
            })
            .Build();
    }

    private void ConfigureHostServices(IServiceCollection services)
    {
        var databaseName = Guid.NewGuid().ToString();
        _emailSender = new Mock<IEmailSender>(MockBehavior.Strict);
        _documentStore = new InMemoryDocumentStore();

        services.AddSingleton(_emailSender.Object);
        services.AddSingleton<IDocumentStore>(_documentStore);
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new EarlyLearnerOptions { Url = new Uri("https://earlylearner.test") }));
        services.AddDbContext<AuditDbContext>(options => options.UseInMemoryDatabase(databaseName));
        services.AddMassTransitTestHarness(configurator => {
            configurator.AddConsumer<HouseholdInvitationEmailRequestedConsumer>();
            configurator.AddConsumer<AuditTrailEntryRecordedConsumer>();
        });
    }

    protected HouseholdInvitationEmailRequestedConsumer CreateHouseholdInvitationEmailRequestedConsumer()
    {
        return new HouseholdInvitationEmailRequestedConsumer(
            _emailSender.Object,
            _documentStore,
            ResolveService<Microsoft.Extensions.Options.IOptions<EarlyLearnerOptions>>());
    }

    protected AuditTrailEntryRecordedConsumer CreateAuditTrailEntryRecordedConsumer()
    {
        return new AuditTrailEntryRecordedConsumer(ResolveService<AuditDbContext>());
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