using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.Common;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.ReadinessContext.Entities;

/// <summary>
/// Aggregate root that summarises a child's whole-child school readiness.
/// Progress remains evidence-based and uses supportive language rather than
/// pass/fail scoring.
/// </summary>
public sealed class ReadinessProfile : Entity<ReadinessProfileId>
{
    private readonly List<ReadinessDomainProgress> _domainProgress = [];
    private readonly List<SuggestedNextStep> _suggestedNextSteps = [];

    private ReadinessProfile(ReadinessProfileId id, HouseholdId householdId, ChildId childId, IEnumerable<ReadinessDomainCode> domainCodes) : base(id)
    {
        HouseholdId = householdId;
        ChildId = childId;
        _domainProgress.AddRange(domainCodes.Select(domainCode => new ReadinessDomainProgress(domainCode)));
    }

    /// <summary>
    /// Household that owns access to this readiness profile.
    /// </summary>
    public HouseholdId HouseholdId { get; }

    /// <summary>
    /// Child whose school-readiness picture is being summarised.
    /// </summary>
    public ChildId ChildId { get; }

    #region Nav props

    /// <summary>
    /// Progress summaries for each tracked readiness domain.
    /// </summary>
    public IReadOnlyCollection<ReadinessDomainProgress> DomainProgress => _domainProgress.AsReadOnly();

    /// <summary>
    /// Practical parent-facing recommendations derived from readiness evidence and gaps.
    /// </summary>
    public IReadOnlyCollection<SuggestedNextStep> SuggestedNextSteps => _suggestedNextSteps.AsReadOnly();

    #endregion

    public static ReadinessProfile Create(
        HouseholdId householdId,
        ChildId childId,
        IEnumerable<ReadinessDomainCode> domainCodes)
    {
        var requiredDomainCodes = domainCodes.Distinct().ToArray();
        if (requiredDomainCodes.Length == 0) throw new DomainException("Readiness profile must track at least one domain.");

        return new ReadinessProfile(
            new ReadinessProfileId(Guid.NewGuid()),
            householdId,
            childId,
            requiredDomainCodes);
    }

    public void AddEvidence(EvidenceReference evidenceReference)
    {
        var progress = _domainProgress.SingleOrDefault(item => item.DomainCode == evidenceReference.DomainCode);
        if (progress is null)
        {
            throw new DomainException("Readiness domain is not tracked by this profile.");
        }

        var previousStatus = progress.Status;
        progress.AddEvidence(evidenceReference);

        RaiseDomainEvent(new ReadinessEvidenceAdded(Id, ChildId, evidenceReference.DomainCode, DateTimeOffset.UtcNow));
        if (previousStatus != progress.Status) RaiseDomainEvent(new ReadinessStatusChanged(Id, ChildId, evidenceReference.DomainCode, progress.Status, DateTimeOffset.UtcNow));
    }

    public SuggestedNextStep AddSuggestedNextStep(ReadinessDomainCode domainCode, string text)
    {
        if (_domainProgress.All(item => item.DomainCode != domainCode)) throw new DomainException("Suggested next step must target a tracked readiness domain.");
        var nextStep = SuggestedNextStep.Create(domainCode, text);
        _suggestedNextSteps.Add(nextStep);
        return nextStep;
    }
}
