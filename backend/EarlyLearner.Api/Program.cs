using EarlyLearner.Api.Configuration;
using EarlyLearner.Api.Configuration.Options;
using EarlyLearner.Api.Endpoints;
using EarlyLearner.Infrastructure.Configuration;
using EarlyLearner.Shared.Observability;
using Serilog;

try {
    var builder = WebApplication.CreateBuilder(args);
    builder.AddSerilog();
    builder.AddEarlyLearnerObservability(builder.Environment, "earlylearner-api", includeAspNetCoreInstrumentation: true);

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

    var corsOptions = builder.Configuration.GetSection(CorsOptions.SECTION_NAME).Get<CorsOptions>()!;

    app.UseHttpsRedirection();
    app.UseCors(corsOptions.PolicyName);
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseAuthentication();
    app.UseMiddleware<UserClaimsMiddleware>();
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
