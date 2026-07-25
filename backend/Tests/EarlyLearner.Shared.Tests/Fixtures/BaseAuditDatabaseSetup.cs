using EarlyLearner.Worker.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NUnit.Framework;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

namespace EarlyLearner.Shared.Tests.Fixtures;

/// <summary>
/// Provides a reusable PostgreSQL audit database fixture for integration-style tests.
/// </summary>
public abstract class BaseAuditDatabaseSetup
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("earlylearner_audit_tests")
        .WithUsername("earlylearner")
        .WithPassword("earlylearner")
        .Build();

    private Respawner _respawner = default!;

    protected AuditDbContext Db { get; private set; } = default!;

    protected string ConnectionString => _postgres.GetConnectionString();

    /// <summary>
    /// Starts the PostgreSQL test container and applies audit database migrations once for the fixture.
    /// </summary>
    [OneTimeSetUp]
    public async Task StartDatabaseAsync()
    {
        await _postgres.StartAsync();

        await using var setupContext = CreateContext();
        await setupContext.Database.MigrateAsync();

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = [new Table("public", "__EFMigrationsHistory")]
        });
    }

    /// <summary>
    /// Resets persisted audit data and creates a fresh database context before each test.
    /// </summary>
    [SetUp]
    public async Task ResetDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
        Db = CreateContext();
    }

    /// <summary>
    /// Disposes the per-test audit database context after each test completes.
    /// </summary>
    [TearDown]
    public async Task DisposeContextAsync()
    {
        if (Db is not null) await Db.DisposeAsync();
    }

    [OneTimeTearDown]
    public async Task StopDatabaseAsync()
    {
        await _postgres.DisposeAsync();
    }

    protected AuditDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AuditDbContext>()
            .UseNpgsql(ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new AuditDbContext(options);
    }
}
