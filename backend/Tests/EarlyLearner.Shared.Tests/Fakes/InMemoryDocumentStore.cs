using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.NotificationService;

namespace EarlyLearner.Shared.Tests.Fakes;

public sealed class InMemoryDocumentStore : IDocumentStore
{
    private readonly Dictionary<(string ContainerName, string Id, string PartitionKey), object> _documents = [];

    public Task EnsureContainerAsync(string containerName, string partitionKeyPath, int? defaultTimeToLiveSeconds = null, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<TDocument?> GetAsync<TDocument>(string containerName, string id, string partitionKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_documents.TryGetValue((containerName, id, partitionKey), out var document) ? (TDocument)document : default);
    }

    public Task UpsertAsync<TDocument>(string containerName, TDocument document, string partitionKey, CancellationToken cancellationToken = default)
    {
        var id = GetDocumentId(document);
        _documents[(containerName, id, partitionKey)] = document!;
        return Task.CompletedTask;
    }

    public Task<bool> TryCreateAsync<TDocument>(string containerName, TDocument document, string partitionKey, CancellationToken cancellationToken = default)
    {
        var id = GetDocumentId(document);
        return Task.FromResult(_documents.TryAdd((containerName, id, partitionKey), document!));
    }

    public Task DeleteAsync(string containerName, string id, string partitionKey, CancellationToken cancellationToken = default)
    {
        _documents.Remove((containerName, id, partitionKey));
        return Task.CompletedTask;
    }

    public NotificationDocument? GetNotification(Guid householdId, Guid invitationId)
    {
        return _documents.TryGetValue((
            NotificationDocument.ContainerName,
            NotificationDocument.BuildId(invitationId),
            NotificationDocument.BuildPartitionKey(householdId)), out var document)
            ? (NotificationDocument)document
            : null;
    }

    private static string GetDocumentId<TDocument>(TDocument document)
    {
        var id = document?.GetType().GetProperty("Id")?.GetValue(document) as string;
        if (string.IsNullOrWhiteSpace(id)) throw new InvalidOperationException($"Document type {typeof(TDocument).Name} does not expose a string Id property.");

        return id;
    }
}
