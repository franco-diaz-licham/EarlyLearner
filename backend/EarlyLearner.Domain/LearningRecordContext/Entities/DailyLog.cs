using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.Common;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.LearningRecordContext.Entities;

/// <summary>
/// Aggregate root for a child's daily learning record. It supports quick parent
/// capture while preserving evidence that can feed readiness progress.
/// </summary>
public sealed class DailyLog : Entity<DailyLogId>
{
    private readonly List<CompletedActivity> _completedActivities = [];
    private readonly List<ReadingEntry> _readingEntries = [];
    private readonly List<RoutineEntry> _routineEntries = [];

    private DailyLog(DailyLogId id, HouseholdId householdId, ChildId childId, DateOnly logDate) : base(id)
    {
        HouseholdId = householdId;
        ChildId = childId;
        LogDate = logDate;
    }

    /// <summary>
    /// Household that owns access to this daily record.
    /// </summary>
    public HouseholdId HouseholdId { get; }

    /// <summary>
    /// Child whose day is being recorded.
    /// </summary>
    public ChildId ChildId { get; }

    /// <summary>
    /// Calendar date the recorded activities and routines happened.
    /// </summary>
    public DateOnly LogDate { get; }

    #region Nav props

    /// <summary>
    /// Learning activities completed on this day that may become readiness evidence.
    /// </summary>
    public IReadOnlyCollection<CompletedActivity> CompletedActivities => _completedActivities.AsReadOnly();

    /// <summary>
    /// Books read with the child and their responses on this day.
    /// </summary>
    public IReadOnlyCollection<ReadingEntry> ReadingEntries => _readingEntries.AsReadOnly();

    /// <summary>
    /// Routines or life skills practised on this day.
    /// </summary>
    public IReadOnlyCollection<RoutineEntry> RoutineEntries => _routineEntries.AsReadOnly();

    #endregion

    public static DailyLog Create(HouseholdId householdId, ChildId childId, DateOnly logDate)
    {
        return new DailyLog(new DailyLogId(Guid.NewGuid()), householdId, childId, logDate);
    }

    public CompletedActivity LogCompletedActivity(string title, IEnumerable<ReadinessDomainCode> readinessDomains)
    {
        var activity = new CompletedActivity(new CompletedActivityId(Guid.NewGuid()), title, readinessDomains);
        _completedActivities.Add(activity);
        RaiseDomainEvent(new LearningActivityLogged(Id, activity.Id, DateTimeOffset.UtcNow));
        return activity;
    }

    public ReadingEntry AddReadingEntry(string title, string author, string childResponse)
    {
        var readingEntry = new ReadingEntry(new ReadingEntryId(Guid.NewGuid()), title, author, childResponse);
        _readingEntries.Add(readingEntry);
        return readingEntry;
    }

    public RoutineEntry AddRoutineEntry(string routineName, string notes)
    {
        var routineEntry = new RoutineEntry(new RoutineEntryId(Guid.NewGuid()), routineName, notes);
        _routineEntries.Add(routineEntry);
        return routineEntry;
    }
}
