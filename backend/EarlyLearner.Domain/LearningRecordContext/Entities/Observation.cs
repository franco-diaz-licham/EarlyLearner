using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.ReadinessContext.Entities;

namespace EarlyLearner.Domain.LearningRecordContext.Entities;

/// <summary>
/// Aggregate root for a meaningful parent observation about the child's
/// learning, behaviour, interests, or development.
/// </summary>
public sealed class Observation : Entity<ObservationId>
{
    private readonly List<ReadinessOutcome> _readinessOutcomes = [];
    private readonly List<StoredFile> _storedFiles = [];

    private Observation(
        ObservationId id,
        HouseholdId householdId,
        ChildId childId,
        DateOnly observedOn,
        string note,
        IEnumerable<ReadinessOutcome> readinessOutcomes)
        : base(id)
    {
        HouseholdId = householdId;
        ChildId = childId;
        ObservedOn = observedOn;
        Note = Required(note, nameof(note));
        var requiredReadinessOutcomes = readinessOutcomes.DistinctBy(outcome => outcome.Id).ToArray();
        if (requiredReadinessOutcomes.Length == 0)
        {
            throw new DomainException("Observation must target at least one readiness outcome.");
        }

        _readinessOutcomes.AddRange(requiredReadinessOutcomes);
    }

    /// <summary>
    /// Household that owns access to this observation.
    /// </summary>
    public HouseholdId HouseholdId { get; }

    /// <summary>
    /// Child the observation is about.
    /// </summary>
    public ChildId ChildId { get; }

    /// <summary>
    /// Date the carer noticed or recorded the learning moment.
    /// </summary>
    public DateOnly ObservedOn { get; }

    /// <summary>
    /// Parent-written description of the behaviour, interest, question, or breakthrough.
    /// </summary>
    public string Note { get; private set; }

    /// <summary>
    /// Readiness areas this observation may support as evidence.
    /// </summary>
    public IReadOnlyCollection<ReadinessOutcome> ReadinessOutcomes => _readinessOutcomes.AsReadOnly();

    #region Nav props

    /// <summary>
    /// Stored files attached to this observation, such as photos, videos, audio notes, or documents.
    /// </summary>
    public IReadOnlyCollection<StoredFile> StoredFiles => _storedFiles.AsReadOnly();

    #endregion

    public static Observation Record(
        HouseholdId householdId,
        ChildId childId,
        DateOnly observedOn,
        string note,
        IEnumerable<ReadinessOutcome> readinessOutcomes)
    {
        var observation = new Observation(
            new ObservationId(Guid.NewGuid()),
            householdId,
            childId,
            observedOn,
            note,
            readinessOutcomes);

        observation.RaiseDomainEvent(new ObservationRecorded(observation.Id, childId, DateTimeOffset.UtcNow));
        return observation;
    }

    public void AttachStoredFile(StoredFile storedFile)
    {
        if (!_storedFiles.Any(file => file.Id == storedFile.Id))
        {
            _storedFiles.Add(storedFile);
        }
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
