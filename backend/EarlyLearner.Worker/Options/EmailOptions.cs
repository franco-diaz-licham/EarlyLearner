using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Worker.Options;

/// <summary>
/// Known email delivery providers supported by the worker.
/// </summary>
public static class EmailProvider
{
    public const string Console = "Console";
    public const string AzureCommunicationServices = "AzureCommunicationServices";
}

/// <summary>
/// Configures how the worker sends outbound emails.
/// </summary>
public sealed class EmailOptions
{
    public const string SECTION_NAME = "Email";

    /// <summary>
    /// Gets the email provider used by the worker.
    /// </summary>
    [Required]
    public string Provider { get; init; } = EmailProvider.Console;

    /// <summary>
    /// Gets the Azure Communication Services connection string.
    /// Required when Provider is AzureCommunicationServices.
    /// </summary>
    public string AzureCommunicationConnectionString { get; init; } = string.Empty;

    /// <summary>
    /// Gets the sender address configured on the verified Azure Communication Services email domain.
    /// Required when Provider is AzureCommunicationServices.
    /// </summary>
    public string SenderAddress { get; init; } = string.Empty;

    public bool UsesAzureCommunicationServices()
    {
        return string.Equals(Provider, EmailProvider.AzureCommunicationServices, StringComparison.OrdinalIgnoreCase);
    }
}
