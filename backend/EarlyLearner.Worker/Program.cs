using EarlyLearner.Worker.Configuration;

var builder = Host.CreateApplicationBuilder(args);
builder.AddHostServices("earlylearner-worker");
WorkerAppServices.AddAppServices(builder.Services, builder.Configuration, builder.Environment);

var host = builder.Build();
host.Run();
