using EarlyLearner.Worker.Configuration;
using Serilog;

try {
    var builder = Host.CreateApplicationBuilder(args);
    builder.AddHostServices("earlylearner-worker");
    WorkerAppServices.AddAppServices(builder.Services, builder.Configuration, builder.Environment);

    var host = builder.Build();
    host.Run();
} catch (Exception ex) {
    Log.Fatal(ex, "Worker startup failed");
    throw;
} finally {
    Log.CloseAndFlush();
}
