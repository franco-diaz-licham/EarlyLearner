using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Worker.Configuration.Options;

/// <summary>
/// Configures the audit database connection used by the worker.
/// </summary>
public sealed class AuditDatabaseOptions
{
    public const string SECTION_NAME = "ConnectionStrings";

    /// <summary>
    /// Gets the connection string for the audit database.
    /// </summary>
    [Required]
    public string AuditDb { get; init; } = string.Empty;
}
