using EarlyLearner.Domain.CoreContext;

namespace EarlyLearner.Application.Ports;

/// <summary>
/// Dispatches domain events raised by aggregates during the current unit of work.
/// </summary>
/// <remarks>
/// Infrastructure implementations can translate domain events into integration events, notifications,
/// or other side effects. When the implementation uses an outbox, dispatch should happen before the
/// unit of work commits so outbox messages are persisted with the aggregate changes.
/// </remarks>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches the collected domain events.
    /// </summary>
    Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken);
}

/// <summary>
/// Handles one domain event type inside the current unit of work.
/// </summary>
public interface IDomainEventHandler
{
    /// <summary>
    /// The domain event type this handler accepts.
    /// </summary>
    Type EventType { get; }

    /// <summary>
    /// Handles the domain event.
    /// </summary>
    Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
}
