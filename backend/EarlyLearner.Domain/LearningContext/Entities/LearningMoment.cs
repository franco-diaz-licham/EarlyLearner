using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;

namespace EarlyLearner.Domain.LearningContext.Entities;

/// <summary>
/// A parent-recorded learning moment owned by a daily log. It captures the
/// concrete evidence that may support one or more learning outcomes.
/// </summary>
public sealed class LearningMoment : Entity<LearningMomentId>
{
    private readonly List<LearningOutcome> _learningOutcomes = [];
    private readonly List<StoredFile> _storedFiles = [];

    private LearningMoment() { }

    internal LearningMoment(
        LearningMomentId id,
        DailyLogId dailyLogId,
        LearningMomentKindEnum kind,
        string title,
        string notes,
        IEnumerable<LearningOutcome> learningOutcomes)
    {
        Id = id;
        DailyLogId = dailyLogId;
        Kind = kind;
        Title = Required(title, nameof(title));
        Notes = Required(notes, nameof(notes));
        var requiredLearningOutcomes = learningOutcomes.DistinctBy(outcome => outcome.Id).ToArray();
        if (requiredLearningOutcomes.Length == 0) throw new DomainException("Learning moment must target at least one learning outcome.");
        _learningOutcomes.AddRange(requiredLearningOutcomes);
        SetCreatedOn();
    }

    public DailyLogId DailyLogId { get; }

    public DailyLog DailyLog { get; private set; } = null!;

    /// <summary>
    /// Broad category for the moment, such as an activity, observation, reading,
    /// or routine.
    /// </summary>
    public LearningMomentKindEnum Kind { get; }

    /// <summary>
    /// Parent-facing name of the captured learning moment.
    /// </summary>
    public string Title { get; } = default!;

    /// <summary>
    /// Parent notes explaining what happened and why the moment matters.
    /// </summary>
    public string Notes { get; } = default!;

    /// <summary>
    /// Learning outcomes this moment practised or demonstrated.
    /// </summary>
    public IReadOnlyCollection<LearningOutcome> LearningOutcomes => _learningOutcomes.AsReadOnly();

    #region Nav props

    /// <summary>
    /// Stored files attached to this learning moment, such as photos, videos, or scanned work.
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
