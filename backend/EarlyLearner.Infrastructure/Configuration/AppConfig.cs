using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EarlyLearner.Infrastructure.Configuration;

public static class AppConfig
{
    private const int DatabaseMigrationMaxAttempts = 10;
    private static readonly TimeSpan DatabaseMigrationInitialRetryDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan DatabaseMigrationMaxRetryDelay = TimeSpan.FromSeconds(30);

    public static async Task ConfigureApp(this IServiceProvider services)
    {
        await services.ConfigureDatabase();
        await services.ConfigureAzureContainer();
    }

    public static async Task ConfigureDatabase(this IServiceProvider services)
    {
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

    public static async Task ConfigureAzureContainer(this IServiceProvider services)
    {
        var container = services.GetRequiredService<BlobContainerClient>();
        await container.CreateIfNotExistsAsync(PublicAccessType.None);
    }
}

