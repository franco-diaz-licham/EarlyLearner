using EarlyLearner.Domain.Common;

namespace EarlyLearner.Domain.ReadinessContext.ValueObjects;

/// <summary>
/// Stable code for a readiness area used to connect plans, logs, observations,
/// goals, and portfolio evidence without relying on display text.
/// </summary>
public sealed record ReadinessDomainCode
{
    private ReadinessDomainCode(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ReadinessDomainCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Readiness domain code is required.");
        }

        return new ReadinessDomainCode(value.Trim().ToLowerInvariant());
    }
}
