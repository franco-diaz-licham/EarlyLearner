using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.ReadinessContext.Entities;

/// <summary>
/// Aggregate root that summarises a child's whole-child school readiness.
/// Progress remains evidence-based and uses supportive language rather than
/// pass/fail scoring.
/// </summary>
public sealed class ReadinessProfile : Entity<ReadinessProfileId>
{
    private readonly List<ReadinessOutcomeProgress> _outcomeProgress = [];

    private ReadinessProfile() { }

    private ReadinessProfile(ReadinessProfileId id, HouseholdId householdId, ChildId childId, IEnumerable<ReadinessOutcome> readinessOutcomes)
    {
        Id = id;
        HouseholdId = householdId;
        ChildId = childId;
        _outcomeProgress.AddRange(readinessOutcomes.Select(readinessOutcome => new ReadinessOutcomeProgress(Id, readinessOutcome)));
        SetCreatedOn();
    }

    /// <summary>
    /// Household that owns access to this readiness profile.
    /// </summary>
    public HouseholdId HouseholdId { get; }

    public Household Household { get; private set; } = null!;

    /// <summary>
    /// Child whose school-readiness picture is being summarised.
    /// </summary>
    public ChildId ChildId { get; }

    public Child Child { get; private set; } = null!;

    #region Nav props

    /// <summary>
    /// Progress summaries for each tracked readiness outcome.
    /// </summary>
    public IReadOnlyCollection<ReadinessOutcomeProgress> OutcomeProgress => _outcomeProgress.AsReadOnly();

    #endregion

    public static ReadinessProfile Create(
        HouseholdId householdId,
        ChildId childId,
        IEnumerable<ReadinessOutcome> readinessOutcomes)
    {
        var requiredReadinessOutcomes = readinessOutcomes.DistinctBy(outcome => outcome.Id).ToArray();
        if (requiredReadinessOutcomes.Length == 0) throw new DomainException("Readiness profile must track at least one outcome.");

        return new ReadinessProfile(
            new ReadinessProfileId(Guid.NewGuid()),
            householdId,
            childId,
            requiredReadinessOutcomes);
    }

    public void AddEvidence(EvidenceReference evidenceReference)
    {
        var progress = _outcomeProgress.SingleOrDefault(item => item.ReadinessOutcome.Id == evidenceReference.ReadinessOutcome.Id);
        if (progress is null) {
            throw new DomainException("readiness outcome is not tracked by this profile.");
        }

        var previousStatus = progress.Status;
        progress.AddEvidence(evidenceReference);

        RaiseDomainEvent(new ReadinessEvidenceAdded(Id, ChildId, evidenceReference.ReadinessOutcome.Id, DateTimeOffset.UtcNow));
        if (previousStatus != progress.Status) RaiseDomainEvent(new ReadinessStatusChanged(Id, ChildId, evidenceReference.ReadinessOutcome.Id, progress.Status, DateTimeOffset.UtcNow));
        SetUpdatedOn();
    }
}
