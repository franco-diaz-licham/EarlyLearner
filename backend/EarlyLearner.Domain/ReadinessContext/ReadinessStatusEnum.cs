namespace EarlyLearner.Domain.ReadinessContext;

/// <summary>
/// Supportive school-readiness status language used across the product instead
/// of pass/fail scoring.
/// </summary>
public enum ReadinessStatusEnum
{
    NotYetObserved = 0,
    Emerging = 1,
    Growing = 2,
    Confident = 3,
    NeedsMoreOpportunities = 4
}
