using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Domain.Common;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.LearningRecordContext.Entities;

/// <summary>
/// A parent-recorded activity completed by the child. It is owned by a daily log
/// and may provide evidence for one or more readiness domains.
/// </summary>
public sealed class CompletedActivity : Entity<CompletedActivityId>
{
    private readonly List<ReadinessDomainCode> _readinessDomains = [];

    internal CompletedActivity(CompletedActivityId id, string title, IEnumerable<ReadinessDomainCode> readinessDomains) : base(id)
    {
        Title = Required(title, nameof(title));
        var requiredReadinessDomains = readinessDomains.Distinct().ToArray();
        if (requiredReadinessDomains.Length == 0) throw new DomainException("Completed activity must target at least one readiness domain.");
        _readinessDomains.AddRange(requiredReadinessDomains);
    }

    /// <summary>
    /// Parent-facing name of the completed activity.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Readiness areas this activity practised or demonstrated.
    /// </summary>
    public IReadOnlyCollection<ReadinessDomainCode> ReadinessDomains => _readinessDomains.AsReadOnly();

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
