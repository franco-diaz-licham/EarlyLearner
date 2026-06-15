namespace EarlyLearner.Domain.LearningRecordContext.ValueObjects;

/// <summary>
/// Identifies an activity completed and recorded in a daily log.
/// </summary>
public readonly record struct CompletedActivityId(Guid Value);
