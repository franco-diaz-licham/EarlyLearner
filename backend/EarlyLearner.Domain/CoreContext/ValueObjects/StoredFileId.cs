namespace EarlyLearner.Domain.CoreContext.ValueObjects;

/// <summary>
/// Identifies a file stored for a household and reused by portfolio, learning,
/// observation, or future assessment records.
/// </summary>
public readonly record struct StoredFileId(Guid Value);
