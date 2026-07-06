namespace EarlyLearner.Shared.Options;

/// <summary>
/// Configures telemetry exporters for local development and cloud hosting.
/// </summary>
public sealed class ObservabilityOptions
{
    public const string SECTION_NAME = "Observability";

    /// <summary>
    /// Gets the local OpenTelemetry collector endpoint used in development.
    /// </summary>
    public string OtlpEndpoint { get; init; } = string.Empty;

    /// <summary>
    /// Gets the Application Insights connection string used outside development.
    /// </summary>
    public string AppInsightConnectionString { get; init; } = string.Empty;
}
