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
        var id = document switch {
            NotificationDocument notification => notification.Id,
            _ => throw new InvalidOperationException($"Document type {typeof(TDocument).Name} is not supported.")
        };

        _documents[(containerName, id, partitionKey)] = document!;
        return Task.CompletedTask;
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
}
