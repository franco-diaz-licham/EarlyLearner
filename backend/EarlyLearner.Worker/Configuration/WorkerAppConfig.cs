using EarlyLearner.Worker.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Worker.Configuration;

public static class WorkerAppConfig
{
    private const int DatabaseMigrationMaxAttempts = 10;
    private static readonly TimeSpan DatabaseMigrationInitialRetryDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan DatabaseMigrationMaxRetryDelay = TimeSpan.FromSeconds(30);

    public static async Task ConfigureDatabase(this IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<AuditDbContext>>();

        for (var attempt = 1; attempt <= DatabaseMigrationMaxAttempts; attempt++) {
            try {
                logger.LogWarning("STARTING AUDIT DATABASE MIGRATION... Attempt {Attempt}/{MaxAttempts}", attempt, DatabaseMigrationMaxAttempts);
                using var scope = services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
                var pendingMigrations = (await db.Database.GetPendingMigrationsAsync()).ToArray();
                logger.LogWarning("PENDING AUDIT DATABASE MIGRATIONS: {PendingMigrations}", pendingMigrations.Length == 0 ? "none" : string.Join(", ", pendingMigrations));
                await db.Database.MigrateAsync();
                logger.LogWarning("AUDIT DATABASE MIGRATION COMPLETED");
                return;
            } catch (Exception exception) when (attempt < DatabaseMigrationMaxAttempts) {
                var delay = GetDatabaseMigrationRetryDelay(attempt);
                logger.LogWarning(exception, "AUDIT DATABASE MIGRATION FAILED. Retrying in {DelaySeconds} seconds...", delay.TotalSeconds);
                await Task.Delay(delay);
            } catch (Exception exception) {
                logger.LogError(exception, "AN ERROR OCCURRED DURING AUDIT DATABASE MIGRATION...");
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
}
