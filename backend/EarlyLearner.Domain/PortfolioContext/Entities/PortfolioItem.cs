using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.Common;
using EarlyLearner.Domain.PortfolioContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.PortfolioContext.Entities;

/// <summary>
/// Aggregate root for evidence a parent chooses to preserve for reflection or
/// future school transition summaries.
/// </summary>
public sealed class PortfolioItem : Entity<PortfolioItemId>
{
    private readonly List<ReadinessDomainCode> _readinessDomains = [];

    private PortfolioItem(
        PortfolioItemId id,
        HouseholdId householdId,
        ChildId childId,
        DateOnly capturedOn,
        string caption,
        AttachmentReference? attachment,
        IEnumerable<ReadinessDomainCode> readinessDomains)
        : base(id)
    {
        HouseholdId = householdId;
        ChildId = childId;
        CapturedOn = capturedOn;
        Caption = Required(caption, nameof(caption));
        Attachment = attachment;
        var requiredReadinessDomains = readinessDomains.Distinct().ToArray();
        if (requiredReadinessDomains.Length == 0)
        {
            throw new DomainException("Portfolio item must target at least one readiness domain.");
        }

        _readinessDomains.AddRange(requiredReadinessDomains);
    }

    /// <summary>
    /// Household that owns access to this portfolio evidence.
    /// </summary>
    public HouseholdId HouseholdId { get; }

    /// <summary>
    /// Child this preserved evidence belongs to.
    /// </summary>
    public ChildId ChildId { get; }

    /// <summary>
    /// Date the evidence was captured or selected for the portfolio.
    /// </summary>
    public DateOnly CapturedOn { get; }

    /// <summary>
    /// Parent-facing note explaining why this item matters.
    /// </summary>
    public string Caption { get; }

    /// <summary>
    /// Optional reference to media or a document stored outside the domain model.
    /// </summary>
    public AttachmentReference? Attachment { get; }

    /// <summary>
    /// Readiness areas this portfolio item may support as evidence.
    /// </summary>
    public IReadOnlyCollection<ReadinessDomainCode> ReadinessDomains => _readinessDomains.AsReadOnly();

    public static PortfolioItem Add(
        HouseholdId householdId,
        ChildId childId,
        DateOnly capturedOn,
        string caption,
        AttachmentReference? attachment,
        IEnumerable<ReadinessDomainCode> readinessDomains)
    {
        var item = new PortfolioItem(
            new PortfolioItemId(Guid.NewGuid()),
            householdId,
            childId,
            capturedOn,
            caption,
            attachment,
            readinessDomains);

        item.RaiseDomainEvent(new PortfolioItemAdded(item.Id, childId, DateTimeOffset.UtcNow));
        return item;
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
