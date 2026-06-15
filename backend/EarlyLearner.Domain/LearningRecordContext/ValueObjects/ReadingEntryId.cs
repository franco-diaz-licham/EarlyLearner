namespace EarlyLearner.Domain.LearningRecordContext.ValueObjects;

/// <summary>
/// Identifies a reading entry recorded in a daily log.
/// </summary>
public readonly record struct ReadingEntryId(Guid Value);
