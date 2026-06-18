using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.ReadinessContext.Entities;

/// <summary>
/// Links a readiness outcome to the concrete evidence that supports progress.
/// Evidence references keep readiness conclusions explainable to parents.
/// </summary>
public sealed class EvidenceReference : Entity<EvidenceReferenceId>
{
    private EvidenceReference() { }

    private EvidenceReference(
        EvidenceReferenceId id,
        ReadinessOutcome readinessOutcome,
        EvidenceSourceTypeEnum sourceType,
        Guid evidenceRecordId,
        DateOnly observedOn,
        string summary)
    {
        Id = id;
        ReadinessOutcomeId = readinessOutcome.Id;
        ReadinessOutcome = readinessOutcome;
        SourceType = sourceType;
        EvidenceRecordId = evidenceRecordId;
        ObservedOn = observedOn;
        Summary = summary;
        SetCreatedOn();
    }

    public int ReadinessOutcomeProgressId { get; private set; }

    public ReadinessOutcomeProgress ReadinessOutcomeProgress { get; private set; } = null!;

    public ReadinessOutcomeId ReadinessOutcomeId { get; }

    /// <summary>
    /// Readiness area supported by this evidence.
    /// </summary>
    public ReadinessOutcome ReadinessOutcome { get; private set; } = default!;

    /// <summary>
    /// Kind of record that produced the evidence.
    /// </summary>
    public EvidenceSourceTypeEnum SourceType { get; }

    /// <summary>
    /// Identifier of the source record at the boundary where evidence was captured.
    /// </summary>
    public Guid EvidenceRecordId { get; }

    /// <summary>
    /// Date the evidence was observed or completed.
    /// </summary>
    public DateOnly ObservedOn { get; }

    /// <summary>
    /// Short explanation shown to parents when reviewing why progress changed.
    /// </summary>
    public string Summary { get; } = default!;

    public static EvidenceReference Create(
        ReadinessOutcome readinessOutcome,
        EvidenceSourceTypeEnum sourceType,
        Guid evidenceRecordId,
        DateOnly observedOn,
        string summary)
    {
        if (evidenceRecordId == Guid.Empty) throw new DomainException("Evidence record id is required.");

        return new EvidenceReference(
            new EvidenceReferenceId(Guid.NewGuid()),
            readinessOutcome,
            sourceType,
            evidenceRecordId,
            observedOn,
            Required(summary, nameof(summary)));
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
