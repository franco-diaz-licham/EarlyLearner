namespace EarlyLearner.Domain.LearningRecordContext.ValueObjects;

/// <summary>
/// Identifies a daily record of completed learning and life activities.
/// </summary>
public readonly record struct DailyLogId(Guid Value);
