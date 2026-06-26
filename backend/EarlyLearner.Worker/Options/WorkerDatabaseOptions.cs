using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Worker.Options;

/// <summary>
/// Configures the worker-owned persistence model.
/// </summary>
public sealed class WorkerDatabaseOptions
{
    public const string SECTION_NAME = "ConnectionStrings";

    [Required] public string Db { get; init; } = string.Empty;
}
