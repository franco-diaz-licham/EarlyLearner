using EarlyLearner.Application.Ports;
using Microsoft.Extensions.Caching.Memory;

namespace EarlyLearner.Api.Configuration;

public sealed class MemoryCachingService(IMemoryCache cache) : ICachingService
{
    public bool TryGetValue<TValue>(string key, out TValue? value)
    {
        return cache.TryGetValue(key, out value);
    }

    public void Set<TValue>(string key, TValue value, TimeSpan absoluteExpirationRelativeToNow)
    {
        cache.Set(key, value, absoluteExpirationRelativeToNow);
    }
}
