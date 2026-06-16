using EarlyLearner.Api.Configuration;
using EarlyLearner.Api.Endpoints;
using EarlyLearner.Infrastructure.Configuration;
using Serilog;

try {
    var builder = WebApplication.CreateBuilder(args);
    builder.AddSerilog();

    try {
        ApiAppServices.AddAppServices(builder);
        InfraAppServices.AddAppServices(builder.Services, builder.Configuration);
    } catch (Exception ex) {
        Log.Fatal(ex, "Exception thrown during InfraAppServices.AddAppServices");
        throw;
    }

    var app = builder.Build();
    if (app.Environment.IsDevelopment()) {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapApiEndpoints();
    await app.Services.ConfigureApp();
    await app.RunAsync();
} catch (Exception ex) {
    Log.Fatal(ex, "Application startup failed");
    throw;
} finally {
    Log.CloseAndFlush();
}
