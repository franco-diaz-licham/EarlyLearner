namespace EarlyLearner.Domain.LearningRecordContext.ValueObjects;

/// <summary>
/// Identifies an observation about a child's learning or development.
/// </summary>
public readonly record struct ObservationId(Guid Value);
