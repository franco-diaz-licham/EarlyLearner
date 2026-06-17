using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Api.Configuration.Options;

public class CorsOptions
{
    public const string SECTION_NAME = "CorsOptions";

    /// <summary>
    /// Named CORS policy used by the API middleware.
    /// </summary>
    [Required] public string PolicyName { get; set; } = null!;

    /// <summary>
    /// Frontend origin allowed to call the API.
    /// </summary>
    [Required] public string Origin { get; set; } = null!;
}
