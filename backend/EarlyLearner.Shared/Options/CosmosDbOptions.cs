using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Shared.Options;

/// <summary>
/// Configures the Azure Cosmos DB document store used by shared document services.
/// </summary>
public sealed class CosmosDbOptions
{
    public const string SECTION_NAME = "CosmosDb";

    /// <summary>
    /// Gets the Cosmos DB account connection string.
    /// Local development typically points this value at the Cosmos DB emulator.
    /// </summary>
    [Required]
    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>
    /// Gets the Cosmos DB database name used by the application document containers.
    /// </summary>
    [Required]
    public string DatabaseName { get; init; } = "earlylearner";

    /// <summary>
    /// Gets the default container time-to-live value, in seconds, applied when containers are created.
    /// </summary>
    [Range(60, int.MaxValue)]
    public int DefaultTimeToLiveSeconds { get; init; } = 86400;

    /// <summary>
    /// Gets a value indicating whether the Cosmos DB emulator's self-signed certificate should be trusted.
    /// This should only be enabled for local development.
    /// </summary>
    public bool AllowInsecureEmulatorCertificate { get; init; }
}
