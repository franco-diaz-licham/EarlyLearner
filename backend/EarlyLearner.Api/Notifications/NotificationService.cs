using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using EarlyLearner.Application.Features.Notifications;
using EarlyLearner.Shared.Documents;
using EarlyLearner.Shared.Notifications;

namespace EarlyLearner.Api.Notifications;

public sealed class NotificationService(IDocumentStore documentStore) : INotificationPublisher, INotificationStream
{
    private readonly ConcurrentDictionary<NotificationSubscriptionKey, ConcurrentDictionary<Guid, Channel<NotificationDto>>> subscribers = new();

    public ValueTask PublishAsync(NotificationDto notification, CancellationToken cancellationToken = default)
    {
        var key = new NotificationSubscriptionKey(notification.HouseholdId, notification.Id);
        if (!subscribers.TryGetValue(key, out var householdSubscribers)) return ValueTask.CompletedTask;

        foreach (var subscriber in householdSubscribers.Values) subscriber.Writer.TryWrite(notification);

        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<NotificationDto> SubscribeAsync(Guid householdId, Guid invitationId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var existingNotification = await GetNotificationAsync(householdId, invitationId, cancellationToken);
        if (existingNotification is not null && existingNotification.IsTerminal) {
            yield return ToDto(existingNotification);
            yield break;
        }

        var subscriberId = Guid.NewGuid();
        var key = new NotificationSubscriptionKey(householdId, invitationId);
        var channel = Channel.CreateUnbounded<NotificationDto>(new UnboundedChannelOptions {
            SingleReader = true,
            SingleWriter = false
        });

        var householdSubscribers = subscribers.GetOrAdd(key, _ => new ConcurrentDictionary<Guid, Channel<NotificationDto>>());
        householdSubscribers[subscriberId] = channel;

        try {
            existingNotification = await GetNotificationAsync(householdId, invitationId, cancellationToken);
            if (existingNotification is not null && existingNotification.IsTerminal) {
                yield return ToDto(existingNotification);
                yield break;
            }

            await foreach (var notification in channel.Reader.ReadAllAsync(cancellationToken)) {
                yield return notification;
                yield break;
            }
        } finally {
            if (subscribers.TryGetValue(key, out householdSubscribers)) {
                householdSubscribers.TryRemove(subscriberId, out _);
                if (householdSubscribers.IsEmpty) subscribers.TryRemove(key, out _);
            }

            channel.Writer.TryComplete();
        }
    }

    private static NotificationDto ToDto(NotificationDocument notification) => new(
        Id: notification.InvitationId,
        HouseholdId: notification.HouseholdId,
        Type: notification.Type,
        Title: notification.Title,
        Message: notification.Message,
        OccurredAt: notification.OccurredAt);

    private async Task<NotificationDocument?> GetNotificationAsync(Guid householdId, Guid invitationId, CancellationToken cancellationToken)
    {
        return await documentStore.GetAsync<NotificationDocument>(
            NotificationDocument.ContainerName,
            NotificationDocument.BuildId(invitationId),
            NotificationDocument.BuildPartitionKey(householdId),
            cancellationToken);
    }

    private readonly record struct NotificationSubscriptionKey(Guid HouseholdId, Guid InvitationId);
}
