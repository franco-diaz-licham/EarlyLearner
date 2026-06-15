namespace EarlyLearner.Domain.LearningRecordContext.ValueObjects;

/// <summary>
/// Identifies a routine entry recorded in a daily log.
/// </summary>
public readonly record struct RoutineEntryId(Guid Value);
