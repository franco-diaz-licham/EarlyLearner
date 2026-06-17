using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace EarlyLearner.Infrastructure.Persistence;

/// <summary>
/// Creates <see cref="DatabaseContext"/> instances for EF Core design-time tooling.
/// </summary>
/// <remarks>
/// The API creates <see cref="DatabaseContext"/> at runtime through dependency injection in
/// <c>InfraAppServices.AddDbServices</c>. EF Core commands such as migrations, database updates,
/// migration scripts, and <c>dbcontext info</c> run before the API host is fully running, so they
/// need an explicit construction path for <see cref="DbContextOptions{TContext}"/>.
/// </remarks>
public sealed class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    /// <summary>
    /// Builds the database context used by EF Core design-time commands.
    /// </summary>
    /// <param name="args">Arguments passed by EF Core tooling. They are not currently required.</param>
    /// <returns>A configured <see cref="DatabaseContext"/> instance for migrations and model inspection.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <c>ConnectionStrings:Db</c> cannot be found in appsettings or environment variables.
    /// </exception>
    public DatabaseContext CreateDbContext(string[] args)
    {
        var apiProjectPath = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "EarlyLearner.Api"));

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(apiProjectPath, "appsettings.json"), optional: false)
            .AddJsonFile(Path.Combine(apiProjectPath, "appsettings.Development.json"), optional: true)
            .Build();

        var connectionString =
            configuration.GetConnectionString("Db") ??
            Environment.GetEnvironmentVariable("ConnectionStrings__Db") ??
            Environment.GetEnvironmentVariable("ConnectionStrings:Db");

        if (string.IsNullOrWhiteSpace(connectionString)) {
            throw new InvalidOperationException("ConnectionStrings:Db is required to create the migrations DbContext.");
        }

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .ConfigureWarnings(warnings => {
                warnings.Ignore(RelationalEventId.OptionalDependentWithoutIdentifyingPropertyWarning);
            })
            .Options;

        return new DatabaseContext(options);
    }
}
