using EarlyLearner.Domain.Common;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.ReadinessContext.Entities;

/// <summary>
/// Tracks one readiness domain within a child's readiness profile. It is owned
/// by the readiness profile aggregate and changes only through the aggregate root.
/// </summary>
public sealed class ReadinessDomainProgress
{
    private readonly List<EvidenceReference> _evidence = [];

    internal ReadinessDomainProgress(ReadinessDomainCode domainCode)
    {
        DomainCode = domainCode;
        Status = ReadinessStatusEnum.NotYetObserved;
    }

    /// <summary>
    /// Readiness area being tracked within the child's profile.
    /// </summary>
    public ReadinessDomainCode DomainCode { get; }

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
        if (evidenceReference.DomainCode != DomainCode) throw new DomainException("Evidence domain must match progress domain.");

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
