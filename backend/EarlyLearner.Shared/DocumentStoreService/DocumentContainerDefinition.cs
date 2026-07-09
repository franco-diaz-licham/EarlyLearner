namespace EarlyLearner.Shared.DocumentStoreService;

/// <summary>
/// Describes a Cosmos DB container that should be ensured during application startup.
/// </summary>
/// <param name="ContainerName">The Cosmos DB container name.</param>
/// <param name="PartitionKeyPath">The container partition key path, for example <c>/householdId</c>.</param>
/// <param name="DefaultTimeToLiveSeconds">The optional default item time-to-live, in seconds.</param>
public sealed record DocumentContainerDefinition(
    string ContainerName,
    string PartitionKeyPath,
    int? DefaultTimeToLiveSeconds = null);
