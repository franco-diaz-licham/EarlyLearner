using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace EarlyLearner.Infrastructure.Configuration;

public static class AppConfig
{
    private const int DatabaseMigrationMaxAttempts = 10;
    private static readonly TimeSpan DatabaseMigrationInitialRetryDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan DatabaseMigrationMaxRetryDelay = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Configures, migrates and seed any data to the database.
    /// </summary>
    public static async Task ConfigureApp(this WebApplication app)
    {
        await app.ConfigureDatabase();
        await app.ConfigureAzureContainer();
    }

    public static async Task ConfigureDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<DatabaseContext>>();

        for (var attempt = 1; attempt <= DatabaseMigrationMaxAttempts; attempt++) {
            try {
                logger.LogWarning("STARTING DATABASE MIGRATION... Attempt {Attempt}/{MaxAttempts}", attempt, DatabaseMigrationMaxAttempts);
                var db = services.GetRequiredService<DatabaseContext>() ?? throw new InvalidOperationException("No data context...");
                var pendingMigrations = (await db.Database.GetPendingMigrationsAsync()).ToArray();
                logger.LogWarning("PENDING DATABASE MIGRATIONS: {PendingMigrations}", pendingMigrations.Length == 0 ? "none" : string.Join(", ", pendingMigrations));
                await db.Database.MigrateAsync();
                logger.LogWarning("DATABASE MIGRATION COMPLETED");
                return;
            } catch (Exception exception) when (attempt < DatabaseMigrationMaxAttempts) {
                var delay = GetDatabaseMigrationRetryDelay(attempt);
                logger.LogWarning(exception, "DATABASE MIGRATION FAILED. Retrying in {DelaySeconds} seconds...", delay.TotalSeconds);
                await Task.Delay(delay);
            } catch (Exception exception) {
                logger.LogError(exception, "AN ERROR OCCURRED DURING DATABASE MIGRATION...");
                throw;
            }
        }
    }

    private static TimeSpan GetDatabaseMigrationRetryDelay(int attempt)
    {
        var delaySeconds = Math.Min(
            DatabaseMigrationInitialRetryDelay.TotalSeconds * Math.Pow(2, attempt - 1),
            DatabaseMigrationMaxRetryDelay.TotalSeconds);

        return TimeSpan.FromSeconds(delaySeconds);
    }

    public static async Task ConfigureAzureContainer(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var container = scope.ServiceProvider.GetRequiredService<BlobContainerClient>();
        await container.CreateIfNotExistsAsync(PublicAccessType.None);
    }
}

