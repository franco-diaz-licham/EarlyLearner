using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Domain.Common;

namespace EarlyLearner.Domain.LearningRecordContext.Entities;

/// <summary>
/// Records a routine or life skill practised by the child, such as dressing,
/// packing a bag, or cleaning up.
/// </summary>
public sealed class RoutineEntry : Entity<RoutineEntryId>
{
    internal RoutineEntry(RoutineEntryId id, string routineName, string notes) : base(id)
    {
        RoutineName = Required(routineName, nameof(routineName));
        Notes = Required(notes, nameof(notes));
    }

    /// <summary>
    /// Name of the routine or life skill practised by the child.
    /// </summary>
    public string RoutineName { get; }

    /// <summary>
    /// Parent notes about how the child approached the routine.
    /// </summary>
    public string Notes { get; }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
