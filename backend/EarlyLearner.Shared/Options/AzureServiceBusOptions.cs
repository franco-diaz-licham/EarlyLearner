using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Shared.Options;

/// <summary>
/// Configures the Azure Service Bus transport used for integration messaging.
/// </summary>
public sealed class AzureServiceBusOptions
{
    public const string SECTION_NAME = "AzureServiceBus";

    /// <summary>
    /// Gets the Service Bus connection string used by local development and emulator runs.
    /// Cloud deployments should prefer managed identity and environment-specific configuration.
    /// </summary>
    [Required]
    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>
    /// Gets the Service Bus administration connection string used for topology operations.
    /// The local emulator exposes administration operations on port 5300.
    /// </summary>
    public string? AdministrationConnectionString { get; init; }

    public int? PrefetchCount { get; init; } = 1;

    public int? ConcurrentMessageLimit { get; init; } = 1;

    public int? TimeoutSeconds { get; init; } = 60;
}
