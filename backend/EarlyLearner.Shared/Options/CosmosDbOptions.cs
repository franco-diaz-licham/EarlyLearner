using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Shared.Options;

public sealed class CosmosDbOptions
{
    public const string SECTION_NAME = "CosmosDb";

    [Required]
    public string ConnectionString { get; init; } = string.Empty;

    [Required]
    public string DatabaseName { get; init; } = "earlylearner";

    [Range(60, int.MaxValue)]
    public int DefaultTimeToLiveSeconds { get; init; } = 86400;

    public bool AllowInsecureEmulatorCertificate { get; init; }
}
