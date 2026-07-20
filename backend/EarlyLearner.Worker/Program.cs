using EarlyLearner.Worker.Configuration;
using Serilog;

try {
    var builder = Host.CreateApplicationBuilder(args);
    builder.AddHostServices("earlylearner-worker");
    WorkerProgram.AddServices(builder.Configuration, builder.Services, builder.Environment);

    var host = builder.Build();
    host.Run();
} catch (Exception ex) {
    Log.Fatal(ex, "Worker startup failed");
    throw;
} finally {
    Log.CloseAndFlush();
}

public static class WorkerProgram
{
    /// <summary>
    /// Registers the worker infrastructure and application services into the DI container.
    /// This keeps the worker composition root reusable by the running worker and in-process tests.
    /// </summary>
    public static IServiceCollection AddServices(IConfiguration configuration, IServiceCollection services, IHostEnvironment environment)
    {
        return WorkerAppServices.AddAppServices(services, configuration, environment);
    }
}
