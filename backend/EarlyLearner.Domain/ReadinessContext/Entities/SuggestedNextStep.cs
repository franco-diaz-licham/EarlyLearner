using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.ReadinessContext.Entities;

/// <summary>
/// A parent-friendly recommendation created from readiness evidence and gaps.
/// It is owned by the readiness profile and should remain practical and calm.
/// </summary>
public sealed class SuggestedNextStep : Entity<SuggestedNextStepId>
{
    private SuggestedNextStep(SuggestedNextStepId id, ReadinessOutcome readinessOutcome, string text) : base(id)
    {
        ReadinessOutcome = readinessOutcome;
        Text = text;
    }

    /// <summary>
    /// Readiness area this recommendation is intended to strengthen.
    /// </summary>
    public ReadinessOutcome ReadinessOutcome { get; }

    /// <summary>
    /// Practical action a carer can try next at home.
    /// </summary>
    public string Text { get; }

    internal static SuggestedNextStep Create(ReadinessOutcome readinessOutcome, string text)
    {
        return new SuggestedNextStep(
            new SuggestedNextStepId(Guid.NewGuid()),
            readinessOutcome,
            Required(text, nameof(text)));
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
