using EarlyLearner.Domain.Common;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.ReadinessContext.Entities;

/// <summary>
/// A parent-friendly recommendation created from readiness evidence and gaps.
/// It is owned by the readiness profile and should remain practical and calm.
/// </summary>
public sealed class SuggestedNextStep : Entity<SuggestedNextStepId>
{
    private SuggestedNextStep(SuggestedNextStepId id, ReadinessDomainCode domainCode, string text) : base(id)
    {
        DomainCode = domainCode;
        Text = text;
    }

    /// <summary>
    /// Readiness area this recommendation is intended to strengthen.
    /// </summary>
    public ReadinessDomainCode DomainCode { get; }

    /// <summary>
    /// Practical action a carer can try next at home.
    /// </summary>
    public string Text { get; }

    internal static SuggestedNextStep Create(ReadinessDomainCode domainCode, string text)
    {
        return new SuggestedNextStep(
            new SuggestedNextStepId(Guid.NewGuid()),
            domainCode,
            Required(text, nameof(text)));
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
