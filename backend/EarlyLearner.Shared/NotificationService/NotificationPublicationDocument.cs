using System.Text.Json.Serialization;

namespace EarlyLearner.Shared.NotificationService;

public sealed record NotificationPublicationDocument(
    [property: JsonPropertyName("id")] string Id,
    Guid HouseholdId,
    Guid InvitationId,
    DateTimeOffset PublishedAt)
{
    public const string ContainerName = "notification-publications";
    public const string PartitionKeyPath = "/householdId";
    public static string BuildId(Guid invitationId) => invitationId.ToString("D");
    public static string BuildPartitionKey(Guid householdId) => householdId.ToString("D");
}
