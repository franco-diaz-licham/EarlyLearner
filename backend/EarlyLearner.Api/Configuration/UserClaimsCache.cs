using EarlyLearner.Application.UseCases.IdentityContext;
using EarlyLearner.Shared.DocumentStoreService;

namespace EarlyLearner.Api.Configuration;

public interface IUserClaimsCache
{
    Task<UserModel?> GetAsync(ExternalUserIdentity identity, CancellationToken cancellationToken = default);

    Task SetAsync(ExternalUserIdentity identity, UserModel user, TimeSpan ttl, CancellationToken cancellationToken = default);
}

public sealed class UserClaimsCache(IDocumentStore documentStore) : IUserClaimsCache
{
    public const string ClaimsContainerName = "user-claims";
    public const string ClaimsPartitionKeyPath = "/partitionKey";

    private const string ClaimsPartitionKey = "claims";

    public async Task<UserModel?> GetAsync(ExternalUserIdentity identity, CancellationToken cancellationToken = default)
    {
        var document = await documentStore.GetAsync<UserClaimsDocument>(
            ClaimsContainerName,
            GetClaimsDocumentId(identity),
            ClaimsPartitionKey,
            cancellationToken);

        return document is null || document.ExpiresAtUtc <= DateTimeOffset.UtcNow
            ? null
            : document.User;
    }

    public Task SetAsync(ExternalUserIdentity identity, UserModel user, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var document = new UserClaimsDocument(
            GetClaimsDocumentId(identity),
            ClaimsPartitionKey,
            user,
            DateTimeOffset.UtcNow.Add(ttl),
            Math.Max(1, (int)Math.Ceiling(ttl.TotalSeconds)));

        return documentStore.UpsertAsync(ClaimsContainerName, document, ClaimsPartitionKey, cancellationToken);
    }

    private static string GetClaimsDocumentId(ExternalUserIdentity identity) => $"{identity.ExternalTenantId ?? "default"}:{identity.ExternalObjectId}";

    private sealed record UserClaimsDocument(
        string Id,
        string PartitionKey,
        UserModel User,
        DateTimeOffset ExpiresAtUtc,
        int Ttl);
}
