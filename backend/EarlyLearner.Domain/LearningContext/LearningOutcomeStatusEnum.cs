namespace EarlyLearner.Domain.LearningContext;

/// <summary>
/// Describes whether a learning outcome is available for new learning moments.
/// </summary>
public enum LearningOutcomeStatusEnum
{
    Active = 1,
    Inactive = 2,
    Archived = 3
}
