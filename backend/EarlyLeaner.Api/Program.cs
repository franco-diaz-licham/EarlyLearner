using EarlyLearner.Api.Configuration;
using EarlyLearner.Infrastructure.Configuration;
using Serilog;

try {
    var builder = WebApplication.CreateBuilder(args);
    builder.AddSerilog();

    try {
        ApiAppServices.AddAppServices(builder);
        // InfraAppServices.AddAppServices(builder);
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
    app.UseRouting();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    // app.UseMiddleware<UserClaimsMiddleware>();
    app.UseAuthorization();
    // app.UseMiddleware<ExceptionMiddleware>();
    // app.MapApiHealthChecks();
    app.MapControllers();
    // await app.ConfigureApp();
    await app.RunAsync();
} catch (Exception ex) {
    Log.Fatal(ex, "Application startup failed");
    throw;
} finally {
    Log.CloseAndFlush();
}
