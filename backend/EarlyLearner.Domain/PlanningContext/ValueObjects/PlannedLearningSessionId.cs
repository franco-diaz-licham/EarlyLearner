namespace EarlyLearner.Domain.PlanningContext.ValueObjects;

/// <summary>
/// Identifies a planned learning session inside a learning plan.
/// </summary>
public readonly record struct PlannedLearningSessionId(Guid Value);
