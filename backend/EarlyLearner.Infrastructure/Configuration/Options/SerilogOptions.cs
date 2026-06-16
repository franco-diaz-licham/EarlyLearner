using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Infrastructure.Configuration.Options;

public sealed class SerilogOptions
{
    public const string SECTION_NAME = "LoggingOptions";

    [Required]
    public string LogFilePath { get; init; } = "Logs\\LogFile.txt";
}
