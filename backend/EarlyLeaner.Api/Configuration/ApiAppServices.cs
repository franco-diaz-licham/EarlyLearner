using EarlyLeaner.Api.Configuration;
using Microsoft.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EarlyLearner.Api.Configuration;

public static class ApiAppServices
{
    public static void AddAppServices(this WebApplicationBuilder builder)
    {
        // Ensure runtime JSON uses enum names (strings) instead of numeric values and omit nulls
        builder.Services.AddControllers();
        builder.Services.AddSwaggerServices();
        // builder.Services.AddApiHealthChecks();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddPermissionAuthorization();
        builder.Services.AddCors(options => {
            options.AddPolicy(name: "AllowAll", configurePolicy: policy => {
                policy.WithOrigins(
                    origins: [
                        "http://localhost:5173",
                        "https://jolly-coast-0a6793b00.6.azurestaticapps.net"
                    ])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        // builder.Services.Configure<ApiBehaviorOptions>(opt => {
        //     opt.InvalidModelStateResponseFactory = actionContext => {
        //         // make all validations errors into array
        //         var errors = actionContext.ModelState
        //                     .Where(e => e.Value?.Errors.Count > 0)
        //                     .SelectMany(x => x.Value?.Errors!)
        //                     .Select(x => x.ErrorMessage).ToArray();

        //         var errorResponse = new ApiValidationErrorResponse(errors: errors);
        //         return new BadRequestObjectResult(error: errorResponse);
        //     };
        // });

        builder.Services
            .AddUseCaseServices()
            .AddRepositoryServices()
            .AddPortServices()
            .AddApiMessagingServices(builder.Configuration);

        builder.Services.AddMemoryCache();
    }

    private static IServiceCollection AddApiMessagingServices(this IServiceCollection services, IConfiguration configuration)
    {

        return services;
    }

    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {

        return services;
    }

    /// <summary>
    /// Configures and adds serilog as a service.
    /// </summary>
    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        // builder.Services
        //     .AddOptions<SerilogOptions>()
        //     .Bind(builder.Configuration.GetSection(key: SerilogOptions.SECTION_NAME))
        //     .ValidateDataAnnotations()
        //     .ValidateOnStart();

        // // Serilog must be configured before the DI container builds, so we resolve
        // // the file path directly here using the bound value from configuration.
        // var env = builder.Environment;
        // var logPath = builder.Configuration
        //     .GetSection(key: SerilogOptions.SECTION_NAME)
        //     .GetValue<string>(key: nameof(SerilogOptions.LogFilePath)) ?? "Logs\\LogFile.txt";
        // var logFile = Path.Combine(path1: env.ContentRootPath, path2: logPath);
        // Directory.CreateDirectory(Path.GetDirectoryName(logFile)!);
        // Log.Logger = new LoggerConfiguration()
        //     .MinimumLevel.Debug()
        //     .MinimumLevel.Override(source: "Microsoft", minimumLevel: LogEventLevel.Warning)
        //     .Enrich.FromLogContext()
        //     .WriteTo.Console()
        //     .WriteTo.File(path: logFile, rollingInterval: RollingInterval.Day)
        //     .CreateLogger();

        // // force serilog to export error file if problems when startup.
        // Serilog.Debugging.SelfLog.Enable(msg => {
        //     File.AppendAllText(path: Path.Combine(path1: env.ContentRootPath, path2: "Logs", path3: "serilog-selflog.txt"), contents: msg);
        // });

        // // Plug Serilog into .NET logging as app initialises.
        // builder.Host.UseSerilog();
    }

    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => {
            options.SwaggerDoc(name: "v1", info: new OpenApiInfo { Title = "Alumno360 API v1" });

            // Use controller name transformed into "Separated Words" for Swagger tags/endpoints
            options.TagActionsBy(apiDesc => {
                if (apiDesc.ActionDescriptor.RouteValues != null &&
                    apiDesc.ActionDescriptor.RouteValues.TryGetValue(key: "controller", value: out var controller) &&
                    !string.IsNullOrEmpty(controller)) {
                    return new List<string> { RouteTransformer.SplitToWords(value: controller) };
                }

                return new List<string> { apiDesc.GroupName ?? apiDesc.ActionDescriptor.DisplayName ?? "default" };
            });
        });
        return services;
    }

    public static IServiceCollection AddUseCaseServices(this IServiceCollection services)
    {


        return services;
    }

    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {

        return services;
    }

    public static IServiceCollection AddPortServices(this IServiceCollection services)
    {

        return services;
    }
}


