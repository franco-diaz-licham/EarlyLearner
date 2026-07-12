namespace EarlyLearner.Shared.DocumentStoreService;

public interface IDocumentStore
{
    Task EnsureContainerAsync(string containerName, string partitionKeyPath, int? defaultTimeToLiveSeconds = null, CancellationToken cancellationToken = default);

    Task<TDocument?> GetAsync<TDocument>(string containerName, string id, string partitionKey, CancellationToken cancellationToken = default);

    Task UpsertAsync<TDocument>(string containerName, TDocument document, string partitionKey, CancellationToken cancellationToken = default);

    Task DeleteAsync(string containerName, string id, string partitionKey, CancellationToken cancellationToken = default);
}
