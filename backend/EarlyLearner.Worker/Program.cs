using EarlyLearner.Worker.Configuration;
using EarlyLearner.Shared.Observability;

var builder = Host.CreateApplicationBuilder(args);
builder.AddEarlyLearnerObservability("earlylearner-worker");
WorkerAppServices.AddAppServices(builder);

var host = builder.Build();
host.Run();
