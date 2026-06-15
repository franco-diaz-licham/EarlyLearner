namespace EarlyLearner.Domain.ReadinessContext;

/// <summary>
/// Describes where a readiness evidence reference originated.
/// </summary>
public enum EvidenceSourceTypeEnum
{
    CompletedActivity = 1,
    ReadingEntry = 2,
    RoutineEntry = 3,
    Observation = 4,
    PortfolioItem = 5,
    Assessment = 6,
    ExtracurricularActivity = 7,
    CommunityEvent = 8
}
