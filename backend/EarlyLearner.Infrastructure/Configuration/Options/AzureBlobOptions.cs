using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Infrastructure.Configuration.Options;

/// <summary>
/// Configures Azure Blob Storage for stored file content.
/// </summary>
public class AzureBlobOptions
{
    public const string SECTION_NAME = "AzureBlob";

    /// <summary>
    /// Gets the Azure Blob Storage connection string.
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// Gets the blob container name used to store uploaded files.
    /// </summary>
    [Required]
    public string ContainerName { get; set; } = null!;
}
