using EarlyLearner.Application.Ports;
using MassTransit;

namespace EarlyLearner.Infrastructure.Messaging;

/// <summary>
/// Publishes integration events through MassTransit.
/// </summary>
/// <remarks>
/// In the API host this runs behind MassTransit's EF Bus Outbox, so publishes made during a unit of
/// work are stored with the same DbContext transaction and delivered by MassTransit after commit.
/// </remarks>
public sealed class IntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    public async ValueTask PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(integrationEvent, integrationEvent.GetType(), cancellationToken);
    }
}
