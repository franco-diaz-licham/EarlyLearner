using EarlyLearner.Domain.PlanningContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.CoreContext;

namespace EarlyLearner.Domain.PlanningContext.Entities;

/// <summary>
/// Aggregate root for a child's intentional plan over a period, usually a week
/// or term. It owns planned sessions and keeps planning changes consistent.
/// </summary>
public sealed class LearningPlan : Entity<LearningPlanId>
{
    private readonly List<PlannedLearningSession> _sessions = [];

    private LearningPlan() { }

    private LearningPlan(
        LearningPlanId id,
        HouseholdId householdId,
        ChildId childId,
        DateRange period,
        string focus)
    {
        Id = id;
        HouseholdId = householdId;
        ChildId = childId;
        Period = period;
        Focus = Required(focus, nameof(focus));
        SetCreatedOn();
    }

    /// <summary>
    /// Household that owns this plan through the child profile.
    /// </summary>
    public HouseholdId HouseholdId { get; }

    public Household Household { get; private set; } = null!;

    /// <summary>
    /// Child the planned learning period is designed for.
    /// </summary>
    public ChildId ChildId { get; }

    public Child Child { get; private set; } = null!;

    /// <summary>
    /// Calendar period covered by this plan.
    /// </summary>
    public DateRange Period { get; private set; } = default!;

    /// <summary>
    /// Parent-friendly statement of the main learning intention for the period.
    /// </summary>
    public string Focus { get; private set; } = default!;

    #region Nav props

    /// <summary>
    /// Planned activities or intentions owned by this learning plan.
    /// </summary>
    public IReadOnlyCollection<PlannedLearningSession> Sessions => _sessions.AsReadOnly();

    #endregion

    public static LearningPlan Create(HouseholdId householdId, ChildId childId, DateRange period, string focus)
    {
        return new LearningPlan(new LearningPlanId(Guid.NewGuid()), householdId, childId, period, focus);
    }

    public void UpdateDetails(DateRange period, string focus)
    {
        Period = period;
        Focus = Required(focus, nameof(focus));
        SetUpdatedOn();
    }

    public PlannedLearningSession AddSession(
        DateOnly plannedDate,
        string title,
        IEnumerable<Goal> goals,
        IEnumerable<ReadinessOutcome> readinessOutcomes)
    {
        if (!Period.Contains(plannedDate)) throw new DomainException("Planned session date must be inside the plan period.");

        var session = new PlannedLearningSession(
            new PlannedLearningSessionId(Guid.NewGuid()),
            Id,
            plannedDate,
            title,
            goals,
            readinessOutcomes);

        _sessions.Add(session);
        return session;
    }

    public void CompleteSession(PlannedLearningSessionId sessionId)
    {
        var session = GetSession(sessionId);
        session.Complete();
    }

    public void SkipSession(PlannedLearningSessionId sessionId)
    {
        var session = GetSession(sessionId);
        session.Skip();
    }

    private PlannedLearningSession GetSession(PlannedLearningSessionId sessionId)
    {
        return _sessions.SingleOrDefault(session => session.Id == sessionId) ?? throw new DomainException("Planned session does not belong to this plan.");
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
