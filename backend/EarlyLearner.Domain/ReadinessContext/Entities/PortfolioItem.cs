using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.ReadinessContext.Entities;

/// <summary>
/// Aggregate root for evidence a parent chooses to preserve for reflection or
/// future school transition summaries.
/// </summary>
public sealed class PortfolioItem : Entity<PortfolioItemId>
{
    private readonly List<ReadinessOutcome> _readinessOutcomes = [];
    private readonly List<StoredFile> _storedFiles = [];

    private PortfolioItem(
        PortfolioItemId id,
        HouseholdId householdId,
        ChildId childId,
        DateOnly capturedOn,
        string caption)
        : base(id)
    {
        HouseholdId = householdId;
        ChildId = childId;
        CapturedOn = capturedOn;
        Caption = caption;
    }

    private PortfolioItem(
        PortfolioItemId id,
        HouseholdId householdId,
        ChildId childId,
        DateOnly capturedOn,
        string caption,
        PortfolioEvidenceSource? source,
        IEnumerable<StoredFile> storedFiles,
        IEnumerable<ReadinessOutcome> readinessOutcomes)
        : base(id)
    {
        HouseholdId = householdId;
        ChildId = childId;
        CapturedOn = capturedOn;
        Caption = Required(caption, nameof(caption));
        Source = source;
        _storedFiles.AddRange(storedFiles.DistinctBy(file => file.Id));
        var requiredReadinessOutcomes = readinessOutcomes.DistinctBy(outcome => outcome.Id).ToArray();
        if (requiredReadinessOutcomes.Length == 0)
        {
            throw new DomainException("Portfolio item must target at least one readiness outcome.");
        }

        _readinessOutcomes.AddRange(requiredReadinessOutcomes);
    }

    /// <summary>
    /// Household that owns access to this portfolio evidence.
    /// </summary>
    public HouseholdId HouseholdId { get; }

    public Household Household { get; private set; } = null!;

    /// <summary>
    /// Child this preserved evidence belongs to.
    /// </summary>
    public ChildId ChildId { get; }

    public Child Child { get; private set; } = null!;

    /// <summary>
    /// Date the evidence was captured or selected for the portfolio.
    /// </summary>
    public DateOnly CapturedOn { get; }

    /// <summary>
    /// Parent-facing note explaining why this item matters.
    /// </summary>
    public string Caption { get; }

    /// <summary>
    /// Optional captured evidence record this portfolio item was selected from.
    /// </summary>
    public PortfolioEvidenceSource? Source { get; private set; }

    /// <summary>
    /// Readiness areas this portfolio item may support as evidence.
    /// </summary>
    public IReadOnlyCollection<ReadinessOutcome> ReadinessOutcomes => _readinessOutcomes.AsReadOnly();

    #region Nav props

    /// <summary>
    /// Stored files selected into this portfolio item, such as photos, videos, artwork, or documents.
    /// </summary>
    public IReadOnlyCollection<StoredFile> StoredFiles => _storedFiles.AsReadOnly();

    #endregion

    public static PortfolioItem Add(
        HouseholdId householdId,
        ChildId childId,
        DateOnly capturedOn,
        string caption,
        PortfolioEvidenceSource? source,
        IEnumerable<StoredFile> storedFiles,
        IEnumerable<ReadinessOutcome> readinessOutcomes)
    {
        var item = new PortfolioItem(
            new PortfolioItemId(Guid.NewGuid()),
            householdId,
            childId,
            capturedOn,
            caption,
            source,
            storedFiles,
            readinessOutcomes);

        item.RaiseDomainEvent(new PortfolioItemAdded(item.Id, childId, DateTimeOffset.UtcNow));
        return item;
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
