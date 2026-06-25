using EarlyLearner.Worker.Configuration;

var builder = Host.CreateApplicationBuilder(args);
WorkerAppServices.AddAppServices(builder);

var host = builder.Build();
host.Run();
