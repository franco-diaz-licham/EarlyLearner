using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Infrastructure.Configuration.Options;

public class AzureBlobOptions
{
    public const string SECTION_NAME = "AzureBlob";

    /// <summary>
    /// Either provide a connection string OR an account Url + managed identity credentials.
    /// </summary>
    [Required] public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// Container name to store files.
    /// </summary>
    [Required] public string ContainerName { get; set; } = null!;
}
