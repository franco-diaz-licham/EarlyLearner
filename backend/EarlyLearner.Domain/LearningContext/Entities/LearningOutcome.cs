using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.LearningContext.ValueObjects;

namespace EarlyLearner.Domain.LearningContext.Entities;

/// <summary>
/// Defines a learning outcome that can be selected when recording learning moments.
/// </summary>
public sealed class LearningOutcome : Entity<LearningOutcomeId>
{
    private LearningOutcome() { }

    private LearningOutcome(
        LearningOutcomeId id,
        string code,
        string name,
        string description,
        string category,
        int sortOrder)
    {
        Id = id;
        Code = Required(code, nameof(code)).ToLowerInvariant();
        Name = Required(name, nameof(name));
        Description = Required(description, nameof(description));
        Category = Required(category, nameof(category));
        SortOrder = sortOrder;
        Status = LearningOutcomeStatusEnum.Active;
        SetCreatedOn();
    }

    /// <summary>
    /// Stable machine-friendly code used for seed data, imports, and display lookup.
    /// </summary>
    public string Code { get; private set; } = default!;

    /// <summary>
    /// Parent-facing label shown when carers select or review this outcome.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Short explanation of the outcome and what evidence may support it.
    /// </summary>
    public string Description { get; private set; } = default!;

    /// <summary>
    /// Higher-level learning area used to group outcomes in the product.
    /// </summary>
    public string Category { get; private set; } = default!;

    /// <summary>
    /// Display order used when presenting outcomes consistently.
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// Lifecycle state that controls whether the outcome can be selected for new records.
    /// </summary>
    public LearningOutcomeStatusEnum Status { get; private set; }

    public static LearningOutcome Create(
        string code,
        string name,
        string description,
        string category,
        int sortOrder)
    {
        return new LearningOutcome(
            new LearningOutcomeId(Guid.NewGuid()),
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
        SetUpdatedOn();
    }

    public void Deactivate()
    {
        Status = LearningOutcomeStatusEnum.Inactive;
        SetUpdatedOn();
    }

    public void Archive()
    {
        Status = LearningOutcomeStatusEnum.Archived;
        SetUpdatedOn();
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) {
            throw new DomainException($"{name} is required.");
        }

        return value.Trim();
    }
}
