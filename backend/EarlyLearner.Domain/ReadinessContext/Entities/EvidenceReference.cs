using EarlyLearner.Domain.Common;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.ReadinessContext.Entities;

/// <summary>
/// Links a readiness domain to the concrete evidence that supports progress.
/// Evidence references keep readiness conclusions explainable to parents.
/// </summary>
public sealed class EvidenceReference : Entity<EvidenceReferenceId>
{
    private EvidenceReference(
        EvidenceReferenceId id,
        ReadinessDomainCode domainCode,
        EvidenceSourceTypeEnum sourceType,
        Guid sourceId,
        DateOnly observedOn,
        string summary)
        : base(id)
    {
        DomainCode = domainCode;
        SourceType = sourceType;
        SourceId = sourceId;
        ObservedOn = observedOn;
        Summary = summary;
    }

    /// <summary>
    /// Readiness area supported by this evidence.
    /// </summary>
    public ReadinessDomainCode DomainCode { get; }

    /// <summary>
    /// Kind of record that produced the evidence.
    /// </summary>
    public EvidenceSourceTypeEnum SourceType { get; }

    /// <summary>
    /// Identifier of the source record at the boundary where evidence was captured.
    /// </summary>
    public Guid SourceId { get; }

    /// <summary>
    /// Date the evidence was observed or completed.
    /// </summary>
    public DateOnly ObservedOn { get; }

    /// <summary>
    /// Short explanation shown to parents when reviewing why progress changed.
    /// </summary>
    public string Summary { get; }

    public static EvidenceReference Create(
        ReadinessDomainCode domainCode,
        EvidenceSourceTypeEnum sourceType,
        Guid sourceId,
        DateOnly observedOn,
        string summary)
    {
        if (sourceId == Guid.Empty) throw new DomainException("Evidence source id is required.");

        return new EvidenceReference(
            new EvidenceReferenceId(Guid.NewGuid()),
            domainCode,
            sourceType,
            sourceId,
            observedOn,
            Required(summary, nameof(summary)));
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");

        return value.Trim();
    }
}
