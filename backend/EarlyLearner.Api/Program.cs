using EarlyLearner.Api.Configuration;
using EarlyLearner.Api.Configuration.Options;
using EarlyLearner.Api.Endpoints;
using EarlyLearner.Infrastructure.Configuration;
using Serilog;

try {
    var builder = WebApplication.CreateBuilder(args);
    builder.AddHostServices("earlylearner-api");

    try {
        ApiAppServices.AddAppServices(builder.Services, builder.Configuration);
        InfraAppServices.AddAppServices(builder.Services, builder.Configuration, builder.Environment);
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

public partial class Program;
