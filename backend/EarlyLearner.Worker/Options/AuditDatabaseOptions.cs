using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Worker.Options;

public sealed class AuditDatabaseOptions
{
    public const string SECTION_NAME = "ConnectionStrings";

    [Required] public string AuditDb { get; init; } = string.Empty;
}
