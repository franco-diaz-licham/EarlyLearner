using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;

namespace EarlyLearner.Domain.LearningRecordContext.Entities;

/// <summary>
/// Records a book read with the child and the child's response. Reading entries
/// are owned by daily logs and can later support literacy evidence.
/// </summary>
public sealed class ReadingEntry : Entity<ReadingEntryId>
{
    private readonly List<StoredFile> _storedFiles = [];

    internal ReadingEntry(ReadingEntryId id, DailyLogId dailyLogId, string title, string author, string childResponse) : base(id)
    {
        DailyLogId = dailyLogId;
        Title = Required(title, nameof(title));
        Author = Required(author, nameof(author));
        ChildResponse = Required(childResponse, nameof(childResponse));
    }

    public DailyLogId DailyLogId { get; }

    public DailyLog DailyLog { get; private set; } = null!;

    /// <summary>
    /// Book title read with the child.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Author credited for the book, used for the reading record and future summaries.
    /// </summary>
    public string Author { get; }

    /// <summary>
    /// Parent-recorded response, question, interest, or comment from the child.
    /// </summary>
    public string ChildResponse { get; }

    #region Nav props

    /// <summary>
    /// Stored files attached to this reading entry, such as a book cover photo or child drawing.
    /// </summary>
    public IReadOnlyCollection<StoredFile> StoredFiles => _storedFiles.AsReadOnly();

    #endregion

    public void AttachStoredFile(StoredFile storedFile)
    {
        if (!_storedFiles.Any(file => file.Id == storedFile.Id))
        {
            _storedFiles.Add(storedFile);
        }
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
