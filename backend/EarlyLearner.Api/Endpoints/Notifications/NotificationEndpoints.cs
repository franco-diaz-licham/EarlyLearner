using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.NotificationService;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Api.Endpoints;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var notifications = endpoints.MapGroup("/notifications").WithTags("Notifications");

        notifications.MapDelete("/{notificationId:guid}", DismissNotification).WithName(nameof(DismissNotification));

        return endpoints;
    }

    public static async Task<IResult> DismissNotification(
        Guid notificationId,
        Guid householdId,
        ICurrentUser currentUser,
        IDocumentStore documentStore,
        CancellationToken cancellationToken = default)
    {
        if (notificationId == Guid.Empty) return Result.Fail("Notification id is required.", ResultTypeEnum.Invalid).ToApiResult();
        if (householdId == Guid.Empty) return Result.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var household = new HouseholdId(householdId);
        if (!currentUser.CanAccess(household)) return Result.Fail("The requested household is not available to the current user.", ResultTypeEnum.Forbidden).ToApiResult();

        await documentStore.DeleteAsync(
            NotificationDocument.ContainerName,
            NotificationDocument.BuildId(notificationId),
            NotificationDocument.BuildPartitionKey(householdId),
            cancellationToken);

        return Result.Success(ResultTypeEnum.Updated).ToApiResult();
    }
}
