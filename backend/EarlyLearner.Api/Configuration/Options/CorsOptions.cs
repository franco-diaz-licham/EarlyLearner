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
    /// Frontend origins allowed to call the API.
    /// </summary>
    [Required, MinLength(1)] public string[] Origins { get; set; } = [];
}
