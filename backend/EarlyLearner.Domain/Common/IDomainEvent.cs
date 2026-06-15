namespace EarlyLearner.Domain.Common;

/// <summary>
/// Marks an in-process event raised by an aggregate after something meaningful
/// happened in the EarlyLearner domain.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// The UTC time at which the domain event was created.
    /// </summary>
    DateTimeOffset OccurredAt { get; }
}
