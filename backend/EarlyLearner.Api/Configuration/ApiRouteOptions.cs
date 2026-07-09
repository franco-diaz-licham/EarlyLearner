namespace EarlyLearner.Api.Configuration;

/// <summary>
/// Defines API route prefix constants used by endpoint mapping.
/// </summary>
public static class ApiRouteOptions
{
    /// <summary>
    /// The current public API version segment.
    /// </summary>
    public const string Version = "v1";

    /// <summary>
    /// The root API route prefix.
    /// </summary>
    public const string ApiPrefix = "api";

    /// <summary>
    /// The combined API route prefix including the version segment.
    /// </summary>
    public const string VersionedApiPrefix = $"{ApiPrefix}/{Version}";
}

