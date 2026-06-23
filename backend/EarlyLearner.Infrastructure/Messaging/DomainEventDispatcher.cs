using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class DomainEventDispatcher(IIntegrationEventPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in domainEvents) {
            if (domainEvent is HouseholdCarerInvited carerInvited) {
                await PublishHouseholdCarerInvitedAsync(carerInvited, cancellationToken);
            }
        }
    }

    private async Task PublishHouseholdCarerInvitedAsync(HouseholdCarerInvited domainEvent, CancellationToken cancellationToken)
    {
        await publisher.PublishAsync(new HouseholdInvitationNotificationRequested(
            Id: Guid.NewGuid(),
            HouseholdId: domainEvent.HouseholdId.Value,
            InvitationId: domainEvent.InvitationId.Value,
            HouseholdName: domainEvent.HouseholdName,
            Email: domainEvent.Email,
            OccurredAt: domainEvent.OccurredAt), cancellationToken);

        await publisher.PublishAsync(new HouseholdInvitationEmailRequested(
            Id: Guid.NewGuid(),
            HouseholdId: domainEvent.HouseholdId.Value,
            InvitationId: domainEvent.InvitationId.Value,
            HouseholdName: domainEvent.HouseholdName,
            Email: domainEvent.Email,
            FirstName: domainEvent.FirstName,
            LastName: domainEvent.LastName,
            ExpiresAt: domainEvent.ExpiresAt,
            OccurredAt: domainEvent.OccurredAt), cancellationToken);
    }
}
