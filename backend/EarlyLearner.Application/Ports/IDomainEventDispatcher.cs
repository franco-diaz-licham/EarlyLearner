using EarlyLearner.Domain.CoreContext;

namespace EarlyLearner.Application.Ports;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken);
}
