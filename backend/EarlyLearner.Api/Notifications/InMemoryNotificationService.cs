using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using EarlyLearner.Application.Features.Notifications;

namespace EarlyLearner.Api.Notifications;

public sealed class InMemoryNotificationService : INotificationPublisher, INotificationStream
{
    private readonly ConcurrentDictionary<Guid, Channel<NotificationDto>> subscribers = new();

    public ValueTask PublishAsync(NotificationDto notification, CancellationToken cancellationToken = default)
    {
        foreach (var subscriber in subscribers.Values) subscriber.Writer.TryWrite(notification);
        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<NotificationDto> SubscribeAsync(Guid householdId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var subscriberId = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<NotificationDto>(new UnboundedChannelOptions {
            SingleReader = true,
            SingleWriter = false
        });

        subscribers[subscriberId] = channel;

        try {
            await foreach (var notification in channel.Reader.ReadAllAsync(cancellationToken)) {
                if (notification.HouseholdId == householdId) yield return notification;
            }
        } finally {
            subscribers.TryRemove(subscriberId, out _);
            channel.Writer.TryComplete();
        }
    }
}
