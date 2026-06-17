using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.ReadinessContext.Entities;

/// <summary>
/// A parent-friendly recommendation created from readiness evidence and gaps.
/// It is owned by the readiness profile and should remain practical and calm.
/// </summary>
public sealed class SuggestedNextStep : Entity<SuggestedNextStepId>
{
    private SuggestedNextStep(SuggestedNextStepId id, ReadinessProfileId readinessProfileId, ReadinessOutcomeId readinessOutcomeId, string text) : base(id)
    {
        ReadinessProfileId = readinessProfileId;
        ReadinessOutcomeId = readinessOutcomeId;
        ReadinessOutcome = null!;
        Text = text;
    }

    private SuggestedNextStep(SuggestedNextStepId id, ReadinessProfileId readinessProfileId, ReadinessOutcome readinessOutcome, string text) : base(id)
    {
        ReadinessProfileId = readinessProfileId;
        ReadinessOutcomeId = readinessOutcome.Id;
        ReadinessOutcome = readinessOutcome;
        Text = text;
    }

    public ReadinessProfileId ReadinessProfileId { get; }

    public ReadinessProfile ReadinessProfile { get; private set; } = null!;

    public ReadinessOutcomeId ReadinessOutcomeId { get; }

    /// <summary>
    /// Readiness area this recommendation is intended to strengthen.
    /// </summary>
    public ReadinessOutcome ReadinessOutcome { get; private set; }

    /// <summary>
    /// Practical action a carer can try next at home.
    /// </summary>
    public string Text { get; }

    internal static SuggestedNextStep Create(ReadinessProfileId readinessProfileId, ReadinessOutcome readinessOutcome, string text)
    {
        return new SuggestedNextStep(
            new SuggestedNextStepId(Guid.NewGuid()),
            readinessProfileId,
            readinessOutcome,
            Required(text, nameof(text)));
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
