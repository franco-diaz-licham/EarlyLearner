namespace EarlyLearner.Api.Configuration.Options;

public sealed class ObservabilityOptions
{
    public const string SECTION_NAME = "Observability";

    public string OtlpEndpoint { get; init; } = string.Empty;

    public string AppInsightConnectionString { get; init; } = string.Empty;
}
