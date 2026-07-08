using Microsoft.Extensions.Hosting;

namespace EarlyLearner.Shared.Documents;

/// <summary>
/// Ensures all registered Cosmos DB document containers exist when the host starts.
/// </summary>
/// <param name="documentStore">The document store used to create or validate containers.</param>
/// <param name="containers">The container definitions registered by the application composition root.</param>
public sealed class DocumentContainerInitializer(IDocumentStore documentStore, IEnumerable<DocumentContainerDefinition> containers) : IHostedService
{
    /// <summary>
    /// Runs once during host startup and ensures each registered container exists.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var container in containers) {
            await documentStore.EnsureContainerAsync(
                container.ContainerName,
                container.PartitionKeyPath,
                container.DefaultTimeToLiveSeconds,
                cancellationToken);
        }
    }

    /// <summary>
    /// Completes immediately because container initialization has no shutdown work.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
