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
    private readonly List<TrackedReadinessOutcome> _trackedOutcomes = [];
    private readonly List<ReadinessEvidence> _evidence = [];

    private ReadinessProfile() { }

    private ReadinessProfile(ReadinessProfileId id, HouseholdId householdId, ChildId childId, IEnumerable<ReadinessOutcome> readinessOutcomes)
    {
        Id = id;
        HouseholdId = householdId;
        ChildId = childId;
        _trackedOutcomes.AddRange(readinessOutcomes.Select(readinessOutcome => new TrackedReadinessOutcome(Id, readinessOutcome)));
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
    /// Readiness outcomes tracked for this child.
    /// </summary>
    public IReadOnlyCollection<TrackedReadinessOutcome> TrackedOutcomes => _trackedOutcomes.AsReadOnly();

    /// <summary>
    /// Evidence that explains why readiness statuses changed.
    /// </summary>
    public IReadOnlyCollection<ReadinessEvidence> Evidence => _evidence.AsReadOnly();

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

    public void AddEvidence(ReadinessEvidence evidence)
    {
        if (evidence.ReadinessProfileId != Id) {
            throw new DomainException("Evidence must belong to this readiness profile.");
        }

        var trackedOutcome = _trackedOutcomes.SingleOrDefault(item => item.ReadinessOutcome.Id == evidence.ReadinessOutcome.Id);
        if (trackedOutcome is null) {
            throw new DomainException("readiness outcome is not tracked by this profile.");
        }

        var previousStatus = trackedOutcome.Status;
        _evidence.Add(evidence);
        var evidenceCount = _evidence.Count(item => item.ReadinessOutcome.Id == trackedOutcome.ReadinessOutcome.Id);
        trackedOutcome.UpdateStatusFromEvidenceCount(evidenceCount);

        RaiseDomainEvent(new ReadinessEvidenceAdded(Id, ChildId, evidence.ReadinessOutcome.Id, DateTimeOffset.UtcNow));
        if (previousStatus != trackedOutcome.Status) RaiseDomainEvent(new ReadinessStatusChanged(Id, ChildId, evidence.ReadinessOutcome.Id, trackedOutcome.Status, DateTimeOffset.UtcNow));
        SetUpdatedOn();
    }
}
