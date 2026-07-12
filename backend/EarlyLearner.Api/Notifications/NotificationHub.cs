using EarlyLearner.Application.Ports;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.NotificationService;
using Microsoft.AspNetCore.SignalR;

namespace EarlyLearner.Api.Notifications;

public sealed class NotificationHub(IDocumentStore documentStore, ICurrentUser currentUser, ILogger<NotificationHub> logger) : Hub
{
    public const string NotificationReceivedMethod = "notification";

    /// <summary>
    /// Subscribes the current SignalR connection to invitation-scoped notifications for the current household.
    /// The frontend calls this after connecting, and again after reconnecting, so invitation email delivery results
    /// can be pushed to the caller. If the notification already reached a terminal state before the subscription
    /// was established, the terminal notification is sent to the caller immediately.
    /// </summary>
    /// <param name="invitationId">The household invitation to receive notification updates for.</param>
    /// <param name="cancellationToken">Cancels the group subscription or notification replay.</param>
    public async Task SubscribeToInvitation(Guid invitationId, CancellationToken cancellationToken = default)
    {
        var householdId = currentUser.HouseholdId.Value;
        var groupName = BuildGroupName(householdId, invitationId);

        logger.LogInformation(
            "Subscribing SignalR connection {ConnectionId} to notification group {GroupName}.",
            Context.ConnectionId,
            groupName);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName, cancellationToken);

        var existingNotification = await GetNotificationAsync(householdId, invitationId, cancellationToken);
        if (existingNotification is not null && existingNotification.IsTerminal) {
            logger.LogInformation(
                "Replaying terminal notification {NotificationId} to SignalR connection {ConnectionId}.",
                existingNotification.Id,
                Context.ConnectionId);

            await Clients.Caller.SendAsync(NotificationReceivedMethod, ToResponse(existingNotification), cancellationToken);
        }
    }

    public static string BuildGroupName(Guid householdId, Guid invitationId) => $"household:{householdId:D}:invitation:{invitationId:D}";

    private static NotificationResponse ToResponse(NotificationDocument notification) => new(
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
