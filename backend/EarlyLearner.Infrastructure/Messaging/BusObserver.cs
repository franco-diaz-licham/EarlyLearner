using MassTransit;
using Microsoft.Extensions.Logging;

namespace EarlyLearner.Infrastructure.Messaging;

public sealed class BusObserver(ILogger<BusObserver> logger) : IBusObserver
{
    public void PostCreate(IBus bus) => logger.LogInformation("Message bus created.");

    public void CreateFaulted(Exception exception) => logger.LogError(exception, "Message bus creation failed.");

    public Task PreStart(IBus bus) => Task.CompletedTask;

    public Task PostStart(IBus bus, Task<BusReady> busReady)
    {
        logger.LogInformation("Message bus started.");
        return Task.CompletedTask;
    }

    public Task StartFaulted(IBus bus, Exception exception)
    {
        logger.LogError(exception, "Message bus start failed.");
        return Task.CompletedTask;
    }

    public Task PreStop(IBus bus) => Task.CompletedTask;

    public Task PostStop(IBus bus)
    {
        logger.LogInformation("Message bus stopped.");
        return Task.CompletedTask;
    }

    public Task StopFaulted(IBus bus, Exception exception)
    {
        logger.LogError(exception, "Message bus stop failed.");
        return Task.CompletedTask;
    }
}
