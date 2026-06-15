namespace EarlyLearner.Domain.PlanningContext.ValueObjects;

/// <summary>
/// Identifies a learning plan for a child and planning period.
/// </summary>
public readonly record struct LearningPlanId(Guid Value);
