using EarlyLearner.Application.Features.Notifications;
using EarlyLearner.Application.Ports;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.NotificationService;
using Microsoft.AspNetCore.SignalR;

namespace EarlyLearner.Api.Notifications;

public sealed class NotificationHub(IDocumentStore documentStore, ICurrentUser currentUser) : Hub
{
    public const string NotificationReceivedMethod = "notification";

    public async Task SubscribeToInvitation(Guid invitationId, CancellationToken cancellationToken = default)
    {
        var householdId = currentUser.HouseholdId.Value;
        await Groups.AddToGroupAsync(Context.ConnectionId, BuildGroupName(householdId, invitationId), cancellationToken);

        var existingNotification = await GetNotificationAsync(householdId, invitationId, cancellationToken);
        if (existingNotification is not null && existingNotification.IsTerminal) {
            await Clients.Caller.SendAsync(NotificationReceivedMethod, ToDto(existingNotification), cancellationToken);
        }
    }

    public Task UnsubscribeFromInvitation(Guid invitationId, CancellationToken cancellationToken = default)
    {
        var householdId = currentUser.HouseholdId.Value;
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, BuildGroupName(householdId, invitationId), cancellationToken);
    }

    public static string BuildGroupName(Guid householdId, Guid invitationId) => $"household:{householdId:D}:invitation:{invitationId:D}";

    private static NotificationDto ToDto(NotificationDocument notification) => new(
        Id: notification.InvitationId,
        HouseholdId: notification.HouseholdId,
        Type: notification.Type,
        Title: notification.Title,
        Message: notification.Message,
        OccurredAt: notification.OccurredAt);

    private Task<NotificationDocument?> GetNotificationAsync(Guid householdId, Guid invitationId, CancellationToken cancellationToken)
    {
        return documentStore.GetAsync<NotificationDocument>(
            NotificationDocument.ContainerName,
            NotificationDocument.BuildId(invitationId),
            NotificationDocument.BuildPartitionKey(householdId),
            cancellationToken);
    }
}
