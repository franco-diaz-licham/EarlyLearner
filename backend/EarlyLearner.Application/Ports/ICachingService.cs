namespace EarlyLearner.Application.Ports;

public interface ICachingService
{
    bool TryGetValue<TValue>(string key, out TValue? value);
    void Set<TValue>(string key, TValue value, TimeSpan absoluteExpirationRelativeToNow);
}
