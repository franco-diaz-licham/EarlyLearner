namespace EarlyLearner.Domain.ReadinessContext;

/// <summary>
/// Describes where a readiness evidence reference originated.
/// </summary>
public enum EvidenceSourceTypeEnum
{
    LearningMoment = 1,
    PortfolioItem = 5,
    Assessment = 6,
    ExtracurricularActivity = 7,
    CommunityEvent = 8
}
