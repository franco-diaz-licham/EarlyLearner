using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.CoreContext;

namespace EarlyLearner.Domain.LearningContext.Entities;

/// <summary>
/// Aggregate root for a child's daily learning record. It supports quick parent
/// capture while preserving evidence that can feed readiness progress.
/// </summary>
public sealed class DailyLog : Entity<DailyLogId>
{
    private readonly List<LearningMoment> _learningMoments = [];

    private DailyLog() { }

    private DailyLog(DailyLogId id, HouseholdId householdId, ChildId childId, DateOnly logDate)
    {
        Id = id;
        HouseholdId = householdId;
        ChildId = childId;
        LogDate = logDate;
        SetCreatedOn();
    }

    /// <summary>
    /// Household that owns access to this daily record.
    /// </summary>
    public HouseholdId HouseholdId { get; }

    public Household Household { get; private set; } = null!;

    /// <summary>
    /// Child whose day is being recorded.
    /// </summary>
    public ChildId ChildId { get; }

    public Child Child { get; private set; } = null!;

    /// <summary>
    /// Calendar date the recorded learning moments happened.
    /// </summary>
    public DateOnly LogDate { get; }

    #region Nav props

    /// <summary>
    /// Learning moments captured on this day that may become readiness evidence.
    /// </summary>
    public IReadOnlyCollection<LearningMoment> LearningMoments => _learningMoments.AsReadOnly();

    #endregion

    public static DailyLog Create(HouseholdId householdId, ChildId childId, DateOnly logDate)
    {
        var dailyLog = new DailyLog(new DailyLogId(Guid.NewGuid()), householdId, childId, logDate);
        var occurredAt = DateTimeOffset.UtcNow;
        dailyLog.RaiseTraceEvent(
            entityName: nameof(DailyLog),
            entityId: dailyLog.Id.Value.ToString(),
            action: "DailyLogCreated",
            summary: "Daily log created",
            details: $"Daily log was created for child {childId.Value} on {logDate}.",
            householdId: householdId.Value,
            occurredAt: occurredAt);
        return dailyLog;
    }

    public LearningMoment RecordLearningMoment(
        LearningMomentKindEnum kind,
        string title,
        string notes,
        IEnumerable<ReadinessOutcome> readinessOutcomes)
    {
        var moment = new LearningMoment(new LearningMomentId(Guid.NewGuid()), Id, kind, title, notes, readinessOutcomes);
        _learningMoments.Add(moment);
        var occurredAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new LearningMomentRecorded(Id, moment.Id, ChildId, occurredAt));
        RaiseTraceEvent(
            entityName: nameof(LearningMoment),
            entityId: moment.Id.Value.ToString(),
            action: "LearningMomentRecorded",
            summary: "Learning moment recorded",
            details: $"{moment.Title} was recorded for child {ChildId.Value}.",
            householdId: HouseholdId.Value,
            occurredAt: occurredAt);
        SetUpdatedOn();
        return moment;
    }
}
