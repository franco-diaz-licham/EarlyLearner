namespace EarlyLearner.Api.Configuration.Options;

/// <summary>
/// Configures observability exporters for the API host.
/// </summary>
public sealed class ObservabilityOptions
{
    public const string SECTION_NAME = "Observability";

    /// <summary>
    /// Gets the OpenTelemetry Protocol endpoint used to export traces and metrics.
    /// </summary>
    public string OtlpEndpoint { get; init; } = string.Empty;

    /// <summary>
    /// Gets the Application Insights connection string used by telemetry exporters.
    /// </summary>
    public string AppInsightConnectionString { get; init; } = string.Empty;
}
