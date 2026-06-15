using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.Common;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.LearningRecordContext.Entities;

/// <summary>
/// Aggregate root for a meaningful parent observation about the child's
/// learning, behaviour, interests, or development.
/// </summary>
public sealed class Observation : Entity<ObservationId>
{
    private readonly List<ReadinessDomainCode> _readinessDomains = [];

    private Observation(
        ObservationId id,
        HouseholdId householdId,
        ChildId childId,
        DateOnly observedOn,
        string note,
        IEnumerable<ReadinessDomainCode> readinessDomains)
        : base(id)
    {
        HouseholdId = householdId;
        ChildId = childId;
        ObservedOn = observedOn;
        Note = Required(note, nameof(note));
        var requiredReadinessDomains = readinessDomains.Distinct().ToArray();
        if (requiredReadinessDomains.Length == 0)
        {
            throw new DomainException("Observation must target at least one readiness domain.");
        }

        _readinessDomains.AddRange(requiredReadinessDomains);
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
    public IReadOnlyCollection<ReadinessDomainCode> ReadinessDomains => _readinessDomains.AsReadOnly();

    public static Observation Record(
        HouseholdId householdId,
        ChildId childId,
        DateOnly observedOn,
        string note,
        IEnumerable<ReadinessDomainCode> readinessDomains)
    {
        var observation = new Observation(
            new ObservationId(Guid.NewGuid()),
            householdId,
            childId,
            observedOn,
            note,
            readinessDomains);

        observation.RaiseDomainEvent(new ObservationRecorded(observation.Id, childId, DateTimeOffset.UtcNow));
        return observation;
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
