using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.ReadinessContext.Entities;

/// <summary>
/// Defines a readiness outcome that can be selected in plans, observations,
/// activities, portfolio items, and readiness tracking.
/// </summary>
public sealed class ReadinessOutcome : Entity<ReadinessOutcomeId>
{
    private ReadinessOutcome(
        ReadinessOutcomeId id,
        string code,
        string name,
        string description,
        string category,
        int sortOrder)
        : base(id)
    {
        Code = Required(code, nameof(code)).ToLowerInvariant();
        Name = Required(name, nameof(name));
        Description = Required(description, nameof(description));
        Category = Required(category, nameof(category));
        SortOrder = sortOrder;
        Status = ReadinessOutcomeStatusEnum.Active;
    }

    /// <summary>
    /// Stable machine-friendly code used for seed data, imports, and display lookup.
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// Parent-facing label shown when carers select or review this outcome.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Short explanation of the outcome and what evidence may support it.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Higher-level readiness area used to group outcomes in the product.
    /// </summary>
    public string Category { get; private set; }

    /// <summary>
    /// Display order used when presenting outcomes consistently.
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// Lifecycle state that controls whether the outcome can be selected for new records.
    /// </summary>
    public ReadinessOutcomeStatusEnum Status { get; private set; }

    public static ReadinessOutcome Create(
        string code,
        string name,
        string description,
        string category,
        int sortOrder)
    {
        return new ReadinessOutcome(
            new ReadinessOutcomeId(Guid.NewGuid()),
            code,
            name,
            description,
            category,
            sortOrder);
    }

    public void UpdateDetails(string name, string description, string category, int sortOrder)
    {
        Name = Required(name, nameof(name));
        Description = Required(description, nameof(description));
        Category = Required(category, nameof(category));
        SortOrder = sortOrder;
    }

    public void Deactivate()
    {
        Status = ReadinessOutcomeStatusEnum.Inactive;
    }

    public void Archive()
    {
        Status = ReadinessOutcomeStatusEnum.Archived;
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) {
            throw new DomainException($"{name} is required.");
        }

        return value.Trim();
    }
}
