using EarlyLearner.Application.Ports;
using MassTransit;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class MassTransitIntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    public async ValueTask PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(integrationEvent, integrationEvent.GetType(), cancellationToken);
    }
}
