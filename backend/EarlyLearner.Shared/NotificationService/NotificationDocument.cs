using System.Text.Json.Serialization;
namespace EarlyLearner.Shared.NotificationService;

public sealed record NotificationDocument(
    [property: JsonPropertyName("id")] string Id,
    Guid HouseholdId,
    Guid InvitationId,
    string Type,
    string Title,
    string Message,
    NotificationDeliveryStatus Status,
    DateTimeOffset OccurredAt)
{
    public bool IsTerminal => Status is NotificationDeliveryStatus.Succeeded or NotificationDeliveryStatus.Failed;
    public const string ContainerName = "notifications";
    public const string PartitionKeyPath = "/householdId";
    public static string BuildId(Guid invitationId) => invitationId.ToString("D");
    public static string BuildPartitionKey(Guid householdId) => householdId.ToString("D");
}
