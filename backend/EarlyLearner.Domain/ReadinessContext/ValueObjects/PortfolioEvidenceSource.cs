using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.ReadinessContext;

namespace EarlyLearner.Domain.ReadinessContext.ValueObjects;

/// <summary>
/// References the captured evidence a carer selected for the portfolio. It lets
/// portfolio items remain curated highlights without duplicating the original
/// learning moment or future assessment record.
/// </summary>
public sealed record PortfolioEvidenceSource
{
    private PortfolioEvidenceSource(EvidenceSourceTypeEnum sourceType, Guid evidenceRecordId, DateOnly sourceDate)
    {
        SourceType = sourceType;
        EvidenceRecordId = evidenceRecordId;
        SourceDate = sourceDate;
    }

    /// <summary>
    /// Kind of captured record selected into the portfolio.
    /// </summary>
    public EvidenceSourceTypeEnum SourceType { get; }

    /// <summary>
    /// Identifier of the captured record selected into the portfolio.
    /// </summary>
    public Guid EvidenceRecordId { get; }

    /// <summary>
    /// Date the original evidence happened or was captured.
    /// </summary>
    public DateOnly SourceDate { get; }

    public static PortfolioEvidenceSource Create(EvidenceSourceTypeEnum sourceType, Guid evidenceRecordId, DateOnly sourceDate)
    {
        if (sourceType == EvidenceSourceTypeEnum.PortfolioItem)
        {
            throw new DomainException("A portfolio item cannot use another portfolio item as its evidence source.");
        }

        if (evidenceRecordId == Guid.Empty)
        {
            throw new DomainException("Portfolio evidence record id is required.");
        }

        return new PortfolioEvidenceSource(sourceType, evidenceRecordId, sourceDate);
    }
}
