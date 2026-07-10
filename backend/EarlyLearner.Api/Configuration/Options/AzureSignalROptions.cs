namespace EarlyLearner.Api.Configuration.Options;

/// <summary>
/// Configures Azure SignalR Service for notification fanout.
/// </summary>
public sealed class AzureSignalROptions
{
    public const string SECTION_NAME = "AzureSignalR";

    /// <summary>
    /// Gets the Azure SignalR Service connection string. When empty, the API uses local SignalR.
    /// </summary>
    public string? ConnectionString { get; init; }
}
