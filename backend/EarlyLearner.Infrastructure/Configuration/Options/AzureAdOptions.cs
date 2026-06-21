using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Infrastructure.Configuration.Options;

public class AzureAdOptions
{
    public const string SECTION_NAME = "AzureAd";

    /// <summary>
    /// Either provide a connection string OR an account Url + managed identity credentials.
    /// </summary>
    [Required] public string Instance { get; set; } = null!;

    /// <summary>
    /// Container name to store files.
    /// </summary>
    [Required] public string TenantId { get; set; } = null!;

    /// <summary>
    /// Container name to store files.
    /// </summary>
    [Required] public string ClientId { get; set; } = null!;
}
