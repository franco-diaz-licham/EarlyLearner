using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Shared.Options;

/// <summary>
/// Configures Serilog file logging options for hosted services.
/// </summary>
public sealed class SerilogOptions
{
    public const string SECTION_NAME = "LoggingOptions";

    /// <summary>
    /// Gets the file path where application logs should be written.
    /// </summary>
    [Required]
    public string LogFilePath { get; init; } = "Logs\\LogFile.txt";
}
