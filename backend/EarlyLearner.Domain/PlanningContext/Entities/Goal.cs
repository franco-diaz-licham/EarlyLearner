using EarlyLearner.Domain.PlanningContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.CoreContext;

namespace EarlyLearner.Domain.PlanningContext.Entities;

/// <summary>
/// Aggregate root representing an intentional learning or development aim for a
/// child. Goals provide context for planning, evidence, and readiness progress.
/// </summary>
public sealed class Goal : Entity<GoalId>
{
    private readonly List<ReadinessOutcome> _readinessOutcomes = [];

    private Goal(
        GoalId id,
        HouseholdId householdId,
        ChildId childId,
        string title,
        GoalTypeEnum type,
        GoalStatusEnum status)
        : base(id)
    {
        HouseholdId = householdId;
        ChildId = childId;
        Title = title;
        Type = type;
        Status = status;
        Timeframe = null!;
    }

    private Goal(
        GoalId id,
        HouseholdId householdId,
        ChildId childId,
        string title,
        GoalTypeEnum type,
        DateRange timeframe,
        IEnumerable<ReadinessOutcome> readinessOutcomes)
        : base(id)
    {
        HouseholdId = householdId;
        ChildId = childId;
        Title = Required(title, nameof(title));
        Type = type;
        Timeframe = timeframe;
        Status = GoalStatusEnum.Active;
        var requiredReadinessOutcomes = readinessOutcomes.DistinctBy(outcome => outcome.Id).ToArray();
        if (requiredReadinessOutcomes.Length == 0)
        {
            throw new DomainException("Goal must target at least one readiness outcome.");
        }

        _readinessOutcomes.AddRange(requiredReadinessOutcomes);
    }

    /// <summary>
    /// Household that owns the child and controls access to this goal.
    /// </summary>
    public HouseholdId HouseholdId { get; }

    public Household Household { get; private set; } = null!;

    /// <summary>
    /// Child this intentional learning or development aim belongs to.
    /// </summary>
    public ChildId ChildId { get; }

    public Child Child { get; private set; } = null!;

    /// <summary>
    /// Parent-facing description of what is being encouraged.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Planning horizon that distinguishes short-term intentions from longer development aims.
    /// </summary>
    public GoalTypeEnum Type { get; }

    /// <summary>
    /// Lifecycle state used to keep active, completed, and archived goals separate.
    /// </summary>
    public GoalStatusEnum Status { get; private set; }

    /// <summary>
    /// Date window in which the carer intends to focus on this goal.
    /// </summary>
    public DateRange Timeframe { get; private set; }

    /// <summary>
    /// School-readiness areas this goal is intended to support.
    /// </summary>
    public IReadOnlyCollection<ReadinessOutcome> ReadinessOutcomes => _readinessOutcomes.AsReadOnly();

    public static Goal Create(
        HouseholdId householdId,
        ChildId childId,
        string title,
        GoalTypeEnum type,
        DateRange timeframe,
        IEnumerable<ReadinessOutcome> readinessOutcomes)
    {
        return new Goal(new GoalId(Guid.NewGuid()), householdId, childId, title, type, timeframe, readinessOutcomes);
    }

    public void Rename(string title)
    {
        Title = Required(title, nameof(title));
    }

    public void Complete()
    {
        if (Status == GoalStatusEnum.Completed) return;
        Status = GoalStatusEnum.Completed;
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
