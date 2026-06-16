using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Infrastructure.Configuration.Options;

public sealed class DatabaseOptions
{
    public const string SECTION_NAME = "ConnectionStrings";

    [Required] public string Db { get; init; } = string.Empty;
}
