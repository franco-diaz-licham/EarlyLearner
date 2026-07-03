namespace EarlyLearner.Domain.ReadinessContext;

/// <summary>
/// Describes whether a readiness outcome is available for new evidence tagging.
/// </summary>
public enum ReadinessOutcomeStatusEnum
{
    Active = 1,
    Inactive = 2,
    Archived = 3
}
