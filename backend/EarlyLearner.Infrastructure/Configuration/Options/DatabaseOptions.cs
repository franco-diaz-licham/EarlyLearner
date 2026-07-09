using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Infrastructure.Configuration.Options;

/// <summary>
/// Configures the primary application database connection.
/// </summary>
public sealed class DatabaseOptions
{
    public const string SECTION_NAME = "ConnectionStrings";

    /// <summary>
    /// Gets the connection string for the primary application database.
    /// </summary>
    [Required]
    public string Db { get; init; } = string.Empty;
}
