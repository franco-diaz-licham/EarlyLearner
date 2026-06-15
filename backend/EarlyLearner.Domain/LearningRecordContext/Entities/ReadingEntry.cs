using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Domain.Common;

namespace EarlyLearner.Domain.LearningRecordContext.Entities;

/// <summary>
/// Records a book read with the child and the child's response. Reading entries
/// are owned by daily logs and can later support literacy evidence.
/// </summary>
public sealed class ReadingEntry : Entity<ReadingEntryId>
{
    internal ReadingEntry(ReadingEntryId id, string title, string author, string childResponse) : base(id)
    {
        Title = Required(title, nameof(title));
        Author = Required(author, nameof(author));
        ChildResponse = Required(childResponse, nameof(childResponse));
    }

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

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
