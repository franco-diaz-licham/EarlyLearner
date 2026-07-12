using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.NotificationService;
using Microsoft.AspNetCore.SignalR;

namespace EarlyLearner.Api.Notifications;

public sealed class NotificationHub(IDocumentStore documentStore, ILogger<NotificationHub> logger) : Hub
{
    /// <summary>
    /// SignalR client method name used when the API pushes a notification payload to connected clients.
    /// Frontend clients must register a handler for this method name to receive notification updates.
    /// </summary>
    public const string NotificationReceivedMethod = "notification";

    public override async Task OnConnectedAsync()
    {
        logger.LogInformation(
            "SignalR: Connection {ConnectionId} connected. Authenticated: {IsAuthenticated}. Household claims: {HouseholdClaimCount}.",
            Context.ConnectionId,
            Context.User?.Identity?.IsAuthenticated == true,
            Context.User?.FindAll(nameof(HouseholdId)).Count() ?? 0);
        await SubscribeFromRequestAsync();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation(exception, "SignalR: Connection {ConnectionId} disconnected.", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Subscribes the current SignalR connection to invitation-scoped notifications for the current household.
    /// The hub uses this during connection setup so invitation email delivery results can be pushed to the caller.
    /// If the notification already reached a terminal state before the subscription was established, the terminal
    /// notification is sent to the caller immediately.
    /// </summary>
    /// <param name="householdId">The household that owns the invitation notification group.</param>
    /// <param name="invitationId">The household invitation to receive notification updates for.</param>
    /// <param name="cancellationToken">Cancels the group subscription or notification replay.</param>
    public async Task SubscribeToInvitation(Guid householdId, Guid invitationId, CancellationToken cancellationToken = default)
    {
        if (!CanAccessHousehold(householdId)) {
            logger.LogWarning("SignalR: Connection {ConnectionId} tried to subscribe to household {HouseholdId}.", Context.ConnectionId, householdId);
            throw new HubException("The requested household is not available to the current user.");
        }

        var groupName = BuildGroupName(householdId, invitationId);
        logger.LogInformation("SignalR: Subscribing connection {ConnectionId} to notification group {GroupName}.", Context.ConnectionId, groupName);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName, cancellationToken);

        var existingNotification = await GetNotificationAsync(householdId, invitationId, cancellationToken);
        if (existingNotification is not null && existingNotification.IsTerminal) {
            logger.LogInformation("SignalR: Replaying terminal notification {NotificationId} to connection {ConnectionId}.", existingNotification.Id, Context.ConnectionId);
            await Clients.Caller.SendAsync(NotificationReceivedMethod, ToResponse(existingNotification), cancellationToken);
        }
    }

    public static string BuildGroupName(Guid householdId, Guid invitationId) => $"household:{householdId:D}:invitation:{invitationId:D}";

    private async Task SubscribeFromRequestAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext is null) return;

        var query = httpContext.Request.Query;
        var hasHouseholdId = Guid.TryParse(query["householdId"], out var householdId);
        var hasInvitationId = Guid.TryParse(query["invitationId"], out var invitationId);

        if (!hasHouseholdId || !hasInvitationId) {
            logger.LogWarning("SignalR connection {ConnectionId} did not include a valid notification subscription.", Context.ConnectionId);
            return;
        }

        await SubscribeToInvitation(householdId, invitationId, httpContext.RequestAborted);
    }

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

    private bool CanAccessHousehold(Guid householdId)
    {
        return Context.User?
            .FindAll(nameof(HouseholdId))
            .Any(claim => Guid.TryParse(claim.Value, out var claimHouseholdId) && claimHouseholdId == householdId) == true;
    }
}
