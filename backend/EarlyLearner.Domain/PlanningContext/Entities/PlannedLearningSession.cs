using EarlyLearner.Domain.PlanningContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.ReadinessContext.Entities;

namespace EarlyLearner.Domain.PlanningContext.Entities;

/// <summary>
/// A scheduled activity or intention inside a learning plan. It is owned by the
/// learning plan aggregate and changed only through that aggregate root.
/// </summary>
public sealed class PlannedLearningSession : Entity<PlannedLearningSessionId>
{
    private readonly List<Goal> _goals = [];
    private readonly List<ReadinessOutcome> _readinessOutcomes = [];

    private PlannedLearningSession(
        PlannedLearningSessionId id,
        LearningPlanId learningPlanId,
        DateOnly plannedDate,
        string title,
        SessionStatusEnum status)
        : base(id)
    {
        LearningPlanId = learningPlanId;
        PlannedDate = plannedDate;
        Title = title;
        Status = status;
    }

    internal PlannedLearningSession(
        PlannedLearningSessionId id,
        LearningPlanId learningPlanId,
        DateOnly plannedDate,
        string title,
        IEnumerable<Goal> goals,
        IEnumerable<ReadinessOutcome> readinessOutcomes)
        : base(id)
    {
        LearningPlanId = learningPlanId;
        PlannedDate = plannedDate;
        Title = Required(title, nameof(title));
        Status = SessionStatusEnum.Planned;
        _goals.AddRange(goals.Distinct());
        var requiredReadinessOutcomes = readinessOutcomes.DistinctBy(outcome => outcome.Id).ToArray();
        if (requiredReadinessOutcomes.Length == 0) throw new DomainException("Planned session must target at least one readiness outcome.");

        _readinessOutcomes.AddRange(requiredReadinessOutcomes);
    }

    public LearningPlanId LearningPlanId { get; }

    public LearningPlan LearningPlan { get; private set; } = null!;

    /// <summary>
    /// Date the carer intends to do this activity or learning moment.
    /// </summary>
    public DateOnly PlannedDate { get; private set; }

    /// <summary>
    /// Parent-facing activity or intention name.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Current lifecycle state of the planned session.
    /// </summary>
    public SessionStatusEnum Status { get; private set; }

    /// <summary>
    /// Goals this planned session is intended to support.
    /// </summary>
    public IReadOnlyCollection<Goal> Goals => _goals.AsReadOnly();

    /// <summary>
    /// Readiness areas expected to be practised or observed during this session.
    /// </summary>
    public IReadOnlyCollection<ReadinessOutcome> ReadinessOutcomes => _readinessOutcomes.AsReadOnly();

    internal void Complete()
    {
        if (Status != SessionStatusEnum.Planned) throw new DomainException("Only planned sessions can be completed.");
        Status = SessionStatusEnum.Completed;
    }

    internal void Skip()
    {
        if (Status != SessionStatusEnum.Planned) throw new DomainException("Only planned sessions can be skipped.");

        Status = SessionStatusEnum.Skipped;
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
