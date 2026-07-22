using EarlyLearner.Worker.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NUnit.Framework;
using Respawn;
using Testcontainers.PostgreSql;

namespace EarlyLearner.Shared.Tests;

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
            SchemasToInclude = ["public"]
        });
    }

    [SetUp]
    public async Task ResetDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
        Db = CreateContext();
    }

    [TearDown]
    public async Task DisposeContextAsync()
    {
        await Db.DisposeAsync();
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