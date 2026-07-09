using EarlyLearner.Shared.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace EarlyLearner.Shared.DocumentStoreService;

public sealed class CosmosDocumentStore(CosmosClient client, IOptions<CosmosDbOptions> options) : IDocumentStore
{
    private readonly CosmosDbOptions options = options.Value;

    public async Task EnsureContainerAsync(string containerName, string partitionKeyPath, int? defaultTimeToLiveSeconds = null, CancellationToken cancellationToken = default)
    {
        var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(options.DatabaseName, cancellationToken: cancellationToken);
        var properties = new ContainerProperties(containerName, partitionKeyPath) {
            DefaultTimeToLive = defaultTimeToLiveSeconds ?? options.DefaultTimeToLiveSeconds
        };

        await databaseResponse.Database.CreateContainerIfNotExistsAsync(properties, cancellationToken: cancellationToken);
    }

    public async Task<TDocument?> GetAsync<TDocument>(string containerName, string id, string partitionKey, CancellationToken cancellationToken = default)
    {
        try {
            var response = await GetContainer(containerName).ReadItemAsync<TDocument>(
                id,
                new PartitionKey(partitionKey),
                cancellationToken: cancellationToken);

            return response.Resource;
        } catch (CosmosException exception) when (exception.StatusCode == System.Net.HttpStatusCode.NotFound) {
            return default;
        }
    }

    public async Task UpsertAsync<TDocument>(string containerName, TDocument document, string partitionKey, CancellationToken cancellationToken = default)
    {
        await GetContainer(containerName).UpsertItemAsync(
            document,
            new PartitionKey(partitionKey),
            cancellationToken: cancellationToken);
    }

    private Container GetContainer(string containerName) => client.GetContainer(options.DatabaseName, containerName);
}
