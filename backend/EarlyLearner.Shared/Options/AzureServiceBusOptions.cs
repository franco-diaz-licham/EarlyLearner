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

    /// <summary>
    /// Gets the number of messages MassTransit should prefetch from Service Bus for each receive endpoint.
    /// Lower values keep local and ordered processing predictable; higher values can improve throughput.
    /// </summary>
    public int? PrefetchCount { get; init; } = 1;

    /// <summary>
    /// Gets the maximum number of messages a receive endpoint may process concurrently.
    /// A value of one keeps message handling serialized for the configured endpoint.
    /// </summary>
    public int? ConcurrentMessageLimit { get; init; } = 1;

    /// <summary>
    /// Gets the MassTransit consume timeout, in seconds, applied to message processing.
    /// Messages that exceed this limit are treated as timed out by the receive pipeline.
    /// </summary>
    public int? TimeoutSeconds { get; init; } = 60;
}
