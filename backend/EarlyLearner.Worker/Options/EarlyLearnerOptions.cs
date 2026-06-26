using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Worker.Options;

/// <summary>
/// Configures public EarlyLearner application links used by worker-generated messages.
/// </summary>
public sealed class EarlyLearnerOptions
{
    public const string SECTION_NAME = "EarlyLearner";

    /// <summary>
    /// Gets the public URL users should open from emails.
    /// </summary>
    [Required]
    public Uri Url { get; init; } = default!;
}
