namespace EarlyLearner.Worker.Configuration.Options;

/// <summary>
/// Configures Azure Communication Services email delivery.
/// </summary>
public sealed class AzureCommunicationServiceOptions
{
    public const string SECTION_NAME = "AzureCommunicationService";

    /// <summary>
    /// Gets the Azure Communication Services connection string.
    /// </summary>
    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>
    /// Gets the sender address configured on the verified Azure Communication Services email domain.
    /// </summary>
    public string SenderAddress { get; init; } = string.Empty;

    /// <summary>
    /// Returns whether the options contain the minimum settings required to send email through Azure Communication Services.
    /// </summary>
    public bool HasRequiredConfiguration()
    {
        return !string.IsNullOrWhiteSpace(ConnectionString) && !string.IsNullOrWhiteSpace(SenderAddress);
    }
}
