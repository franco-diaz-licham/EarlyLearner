using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;

namespace EarlyLearner.Domain.LearningContext.Entities;

/// <summary>
/// Aggregate root for a child's daily learning record. It supports quick parent
/// capture while preserving evidence that can feed readiness progress.
/// </summary>
public sealed class DailyLog : Entity<DailyLogId>
{
    private readonly List<CompletedActivity> _completedActivities = [];
    private readonly List<ReadingEntry> _readingEntries = [];
    private readonly List<RoutineEntry> _routineEntries = [];
    private readonly List<StoredFile> _storedFiles = [];

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

    /// <summary>
    /// Stored files attached to the whole daily record, such as a photo summary or scanned worksheet.
    /// </summary>
    public IReadOnlyCollection<StoredFile> StoredFiles => _storedFiles.AsReadOnly();

    #endregion

    public static DailyLog Create(HouseholdId householdId, ChildId childId, DateOnly logDate)
    {
        return new DailyLog(new DailyLogId(Guid.NewGuid()), householdId, childId, logDate);
    }

    public CompletedActivity LogCompletedActivity(string title, IEnumerable<ReadinessOutcome> readinessOutcomes)
    {
        var activity = new CompletedActivity(new CompletedActivityId(Guid.NewGuid()), Id, title, readinessOutcomes);
        _completedActivities.Add(activity);
        RaiseDomainEvent(new LearningActivityLogged(Id, activity.Id, DateTimeOffset.UtcNow));
        SetUpdatedOn();
        return activity;
    }

    public ReadingEntry AddReadingEntry(string title, string author, string childResponse)
    {
        var readingEntry = new ReadingEntry(new ReadingEntryId(Guid.NewGuid()), Id, title, author, childResponse);
        _readingEntries.Add(readingEntry);
        SetUpdatedOn();
        return readingEntry;
    }

    public RoutineEntry AddRoutineEntry(string routineName, string notes)
    {
        var routineEntry = new RoutineEntry(new RoutineEntryId(Guid.NewGuid()), Id, routineName, notes);
        _routineEntries.Add(routineEntry);
        SetUpdatedOn();
        return routineEntry;
    }

    public void AttachStoredFile(StoredFile storedFile)
    {
        if (!_storedFiles.Any(file => file.Id == storedFile.Id)) {
            _storedFiles.Add(storedFile);
            SetUpdatedOn();
        }
    }
}
