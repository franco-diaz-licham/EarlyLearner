using EarlyLearner.Application.Ports;
using EarlyLearner.Shared.Messaging;

namespace EarlyLearner.Shared.Tests.Fakes;

public sealed class InMemoryIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly List<IIntegrationEvent> _publishedEvents = [];

    public IReadOnlyCollection<IIntegrationEvent> PublishedEvents => _publishedEvents.AsReadOnly();

    public ValueTask PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        _publishedEvents.Add(integrationEvent);
        return ValueTask.CompletedTask;
    }
}
