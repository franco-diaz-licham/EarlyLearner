using EarlyLearner.Domain.CoreContext;
namespace EarlyLearner.Domain.ReadinessContext.Entities;

/// <summary>
/// Tracks one readiness outcome within a child's readiness profile. It is owned
/// by the readiness profile aggregate and changes only through the aggregate root.
/// </summary>
public sealed class ReadinessOutcomeProgress
{
    private readonly List<EvidenceReference> _evidence = [];

    internal ReadinessOutcomeProgress(ReadinessOutcome readinessOutcome)
    {
        ReadinessOutcome = readinessOutcome;
        Status = ReadinessStatusEnum.NotYetObserved;
    }

    /// <summary>
    /// Readiness area being tracked within the child's profile.
    /// </summary>
    public ReadinessOutcome ReadinessOutcome { get; }

    /// <summary>
    /// Supportive progress status calculated from the available evidence.
    /// </summary>
    public ReadinessStatusEnum Status { get; private set; }

    #region Nav props

    /// <summary>
    /// Evidence references that explain why this readiness status was reached.
    /// </summary>
    public IReadOnlyCollection<EvidenceReference> Evidence => _evidence.AsReadOnly();

    #endregion

    internal void AddEvidence(EvidenceReference evidenceReference)
    {
        if (evidenceReference.ReadinessOutcome.Id != ReadinessOutcome.Id) throw new DomainException("Evidence domain must match progress outcome.");

        _evidence.Add(evidenceReference);
        Status = CalculateStatus();
    }

    private ReadinessStatusEnum CalculateStatus()
    {
        return _evidence.Count switch
        {
            0 => ReadinessStatusEnum.NotYetObserved,
            1 => ReadinessStatusEnum.Emerging,
            <= 3 => ReadinessStatusEnum.Growing,
            _ => ReadinessStatusEnum.Confident
        };
    }
}
