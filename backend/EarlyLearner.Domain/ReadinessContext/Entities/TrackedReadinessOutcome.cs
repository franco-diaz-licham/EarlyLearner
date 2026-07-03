using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.ReadinessContext.Entities;

/// <summary>
/// Tracks one readiness outcome within a child's readiness profile.
/// </summary>
public sealed class TrackedReadinessOutcome
{
    private TrackedReadinessOutcome() { }

    internal TrackedReadinessOutcome(ReadinessProfileId readinessProfileId, ReadinessOutcome readinessOutcome)
    {
        ReadinessProfileId = readinessProfileId;
        ReadinessOutcomeId = readinessOutcome.Id;
        ReadinessOutcome = readinessOutcome;
        Status = ReadinessStatusEnum.NotYetObserved;
    }

    public int Id { get; private set; }

    public ReadinessProfileId ReadinessProfileId { get; }

    public ReadinessProfile ReadinessProfile { get; private set; } = null!;

    public ReadinessOutcomeId ReadinessOutcomeId { get; }

    /// <summary>
    /// Readiness area being tracked within the child's profile.
    /// </summary>
    public ReadinessOutcome ReadinessOutcome { get; private set; } = null!;

    /// <summary>
    /// Supportive progress status calculated from the available evidence.
    /// </summary>
    public ReadinessStatusEnum Status { get; private set; }

    internal void UpdateStatusFromEvidenceCount(int evidenceCount)
    {
        if (evidenceCount < 0) throw new DomainException("Evidence count cannot be negative.");

        Status = evidenceCount switch {
            0 => ReadinessStatusEnum.NotYetObserved,
            1 => ReadinessStatusEnum.Emerging,
            <= 3 => ReadinessStatusEnum.Growing,
            _ => ReadinessStatusEnum.Confident
        };
    }
}
