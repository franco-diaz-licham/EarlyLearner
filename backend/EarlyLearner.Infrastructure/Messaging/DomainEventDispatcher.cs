using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class DomainEventDispatcher(IEnumerable<IDomainEventHandler> domainEventHandlers) : IDomainEventDispatcher
{
    private readonly IReadOnlyDictionary<Type, IReadOnlyList<IDomainEventHandler>> handlersByEventType = domainEventHandlers
        .GroupBy(handler => handler.EventType)
        .ToDictionary(group => group.Key, group => (IReadOnlyList<IDomainEventHandler>)group.ToList());

    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in domainEvents) {
            if (!handlersByEventType.TryGetValue(domainEvent.GetType(), out var handlers)) continue;
            foreach (var handler in handlers) await handler.HandleAsync(domainEvent, cancellationToken);
        }
    }
}
