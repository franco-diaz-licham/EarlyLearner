namespace EarlyLearner.Infrastructure.Configuration.Options;

public sealed class EncryptionOptions
{
    public const string SECTION_NAME = "Encryption";

    /// <summary>
    /// Set to false in local development to skip encryption entirely.
    /// Must be true in all non-local environments.
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// Base64-encoded 32-byte AES-256 key. Required when IsEnabled is true.
    /// </summary>
    public string Key { get; init; } = string.Empty;
}
