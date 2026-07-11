using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Worker.Configuration.Options;

/// <summary>
/// Configures Serilog file logging for the worker host.
/// </summary>
public sealed class SerilogOptions
{
    public const string SECTION_NAME = "LoggingOptions";

    /// <summary>
    /// Gets the file path where worker logs should be written.
    /// </summary>
    [Required]
    public string LogFilePath { get; init; } = "Logs\\LogFile.txt";
}
