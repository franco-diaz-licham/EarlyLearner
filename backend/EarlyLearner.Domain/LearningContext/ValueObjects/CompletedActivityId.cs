namespace EarlyLearner.Domain.LearningContext.ValueObjects;

/// <summary>
/// Identifies an activity completed and recorded in a daily log.
/// </summary>
public readonly record struct CompletedActivityId(Guid Value);
