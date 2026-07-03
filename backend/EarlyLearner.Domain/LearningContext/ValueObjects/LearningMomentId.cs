namespace EarlyLearner.Domain.LearningContext.ValueObjects;

/// <summary>
/// Identifies a learning moment recorded in a daily log.
/// </summary>
public readonly record struct LearningMomentId(Guid Value);
