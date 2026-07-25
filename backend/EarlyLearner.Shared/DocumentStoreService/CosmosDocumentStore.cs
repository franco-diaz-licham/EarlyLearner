using System.Net;
using System.Text.Json;
using EarlyLearner.Shared.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace EarlyLearner.Shared.DocumentStoreService;

public sealed class CosmosDocumentStore(CosmosClient client, IOptions<CosmosDbOptions> options) : IDocumentStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly CosmosDbOptions options = options.Value;

    public async Task EnsureContainerAsync(
        string containerName,
        string partitionKeyPath,
        int? defaultTimeToLiveSeconds = null,
        CancellationToken cancellationToken = default)
    {
        var databaseResponse =
            await client.CreateDatabaseIfNotExistsAsync(options.DatabaseName, throughput: options.Throughput, cancellationToken: cancellationToken);
        var properties = new ContainerProperties(containerName, partitionKeyPath) { DefaultTimeToLive = defaultTimeToLiveSeconds ?? options.DefaultTimeToLiveSeconds };
        await databaseResponse.Database.CreateContainerIfNotExistsAsync(properties, cancellationToken: cancellationToken);
    }

    public async Task<TDocument?> GetAsync<TDocument>(string containerName, string id, string partitionKey, CancellationToken cancellationToken = default)
    {
        using var response = await GetContainer(containerName).ReadItemStreamAsync(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return default;
        response.EnsureSuccessStatusCode();
        return await JsonSerializer.DeserializeAsync<TDocument>(response.Content, SerializerOptions, cancellationToken);
    }

    public async Task UpsertAsync<TDocument>(string containerName, TDocument document, string partitionKey, CancellationToken cancellationToken = default)
    {
        await GetContainer(containerName).UpsertItemAsync(document, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
    }

    public async Task<bool> TryCreateAsync<TDocument>(string containerName, TDocument document, string partitionKey, CancellationToken cancellationToken = default)
    {
        try {
            await GetContainer(containerName).CreateItemAsync(document, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
            return true;
        } catch (CosmosException exception) when (exception.StatusCode == HttpStatusCode.Conflict) {
            return false;
        }
    }

    public async Task DeleteAsync(string containerName, string id, string partitionKey, CancellationToken cancellationToken = default)
    {
        try {
            await GetContainer(containerName).DeleteItemAsync<object>(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
        } catch (CosmosException exception) when (exception.StatusCode == HttpStatusCode.NotFound) {
        }
    }

    private Container GetContainer(string containerName) => client.GetContainer(options.DatabaseName, containerName);
}
