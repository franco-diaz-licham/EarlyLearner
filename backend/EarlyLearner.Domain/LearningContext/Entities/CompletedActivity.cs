using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.ReadinessContext.Entities;

namespace EarlyLearner.Domain.LearningContext.Entities;

/// <summary>
/// A parent-recorded activity completed by the child. It is owned by a daily log
/// and may provide evidence for one or more readiness outcomes.
/// </summary>
public sealed class CompletedActivity : Entity<CompletedActivityId>
{
    private readonly List<ReadinessOutcome> _readinessOutcomes = [];
    private readonly List<StoredFile> _storedFiles = [];

    private CompletedActivity() { }

    internal CompletedActivity(CompletedActivityId id, DailyLogId dailyLogId, string title, IEnumerable<ReadinessOutcome> readinessOutcomes)
    {
        Id = id;
        DailyLogId = dailyLogId;
        Title = Required(title, nameof(title));
        var requiredReadinessOutcomes = readinessOutcomes.DistinctBy(outcome => outcome.Id).ToArray();
        if (requiredReadinessOutcomes.Length == 0) throw new DomainException("Completed activity must target at least one readiness outcome.");
        _readinessOutcomes.AddRange(requiredReadinessOutcomes);
        SetCreatedOn();
    }

    public DailyLogId DailyLogId { get; }

    public DailyLog DailyLog { get; private set; } = null!;

    /// <summary>
    /// Parent-facing name of the completed activity.
    /// </summary>
    public string Title { get; } = default!;

    /// <summary>
    /// Readiness areas this activity practised or demonstrated.
    /// </summary>
    public IReadOnlyCollection<ReadinessOutcome> ReadinessOutcomes => _readinessOutcomes.AsReadOnly();

    #region Nav props

    /// <summary>
    /// Stored files attached to this completed activity, such as photos, videos, or scanned work.
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
