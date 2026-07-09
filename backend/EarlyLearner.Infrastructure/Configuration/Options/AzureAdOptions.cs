using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Infrastructure.Configuration.Options;

/// <summary>
/// Configures Microsoft Entra ID authentication for the API.
/// </summary>
public class AzureAdOptions
{
    public const string SECTION_NAME = "AzureAd";

    /// <summary>
    /// Gets the Microsoft identity platform instance URL, such as https://login.microsoftonline.com/.
    /// </summary>
    [Required]
    public string Instance { get; set; } = null!;

    /// <summary>
    /// Gets the tenant identifier accepted by the API.
    /// </summary>
    [Required]
    public string TenantId { get; set; } = null!;

    /// <summary>
    /// Gets the application client identifier registered for the API.
    /// </summary>
    [Required]
    public string ClientId { get; set; } = null!;
}
