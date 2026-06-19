using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;

namespace EarlyLearner.Domain.LearningContext.Entities;

/// <summary>
/// Records a routine or life skill practised by the child, such as dressing,
/// packing a bag, or cleaning up.
/// </summary>
public sealed class RoutineEntry : Entity<RoutineEntryId>
{
    private readonly List<StoredFile> _storedFiles = [];

    private RoutineEntry() { }

    internal RoutineEntry(RoutineEntryId id, DailyLogId dailyLogId, string routineName, string notes)
    {
        Id = id;
        DailyLogId = dailyLogId;
        RoutineName = Required(routineName, nameof(routineName));
        Notes = Required(notes, nameof(notes));
        SetCreatedOn();
    }

    public DailyLogId DailyLogId { get; }

    public DailyLog DailyLog { get; private set; } = null!;

    /// <summary>
    /// Name of the routine or life skill practised by the child.
    /// </summary>
    public string RoutineName { get; } = default!;

    /// <summary>
    /// Parent notes about how the child approached the routine.
    /// </summary>
    public string Notes { get; } = default!;

    #region Nav props

    /// <summary>
    /// Stored files attached to this routine entry, such as photos or short videos of the skill practised.
    /// </summary>
    public IReadOnlyCollection<StoredFile> StoredFiles => _storedFiles.AsReadOnly();

    #endregion

    public void AttachStoredFile(StoredFile storedFile)
    {
        if (!_storedFiles.Any(file => file.Id == storedFile.Id)) {
            _storedFiles.Add(storedFile);
            SetUpdatedOn();
        }
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
