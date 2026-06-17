using EarlyLearner.Api.Configuration.Options;
using EarlyLearner.Api.Endpoints;
using EarlyLearner.Infrastructure.Configuration.Options;
using FluentValidation;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Events;

namespace EarlyLearner.Api.Configuration;

public static class ApiAppServices
{
    public static void AddAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerServices();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddPermissionAuthorization();
        builder.Services.AddRequestValidators();

        builder.Services
            .AddUseCaseServices()
            .AddRepositoryServices()
            .AddPortServices()
            .AddCorsPolicy(builder.Configuration)
            .AddApiMessagingServices(builder.Configuration);

        builder.Services.AddMemoryCache();
    }

    private static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<CorsOptions>()
            .Bind(configuration.GetSection(CorsOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var corsOptions = configuration.GetSection(CorsOptions.SECTION_NAME).Get<CorsOptions>()!;

        services.AddCors(options => {
            options.AddPolicy(name: corsOptions.PolicyName, configurePolicy: policy => {
                policy.WithOrigins(
                    origins: [
                        corsOptions.Origin,
                    ])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
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
        builder.Services
            .AddOptions<SerilogOptions>()
            .Bind(builder.Configuration.GetSection(key: SerilogOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Serilog must be configured before the DI container builds, so we resolve
        // the file path directly here using the bound value from configuration.
        var env = builder.Environment;
        var logPath = builder.Configuration
            .GetSection(key: SerilogOptions.SECTION_NAME)
            .GetValue<string>(key: nameof(SerilogOptions.LogFilePath)) ?? "Logs\\LogFile.txt";
        var logFile = Path.Combine(path1: env.ContentRootPath, path2: logPath);
        Directory.CreateDirectory(Path.GetDirectoryName(logFile)!);
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override(source: "Microsoft", minimumLevel: LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(path: logFile, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // force serilog to export error file if problems when startup.
        Serilog.Debugging.SelfLog.Enable(msg => {
            File.AppendAllText(path: Path.Combine(path1: env.ContentRootPath, path2: "Logs", path3: "serilog-selflog.txt"), contents: msg);
        });

        // Plug Serilog into .NET logging as app initialises.
        builder.Host.UseSerilog();
    }

    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => {
            options.SwaggerDoc(name: "v1", info: new OpenApiInfo { Title = "EarlyLearner API v1" });
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

    private static IServiceCollection AddRequestValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateStoredFileRequest>, CreateStoredFileRequestValidator>();
        services.AddScoped<IValidator<UpdateStoredFileStatusRequest>, UpdateStoredFileStatusRequestValidator>();
        services.AddScoped<IValidator<CreateHouseholdRequest>, CreateHouseholdRequestValidator>();
        services.AddScoped<IValidator<UpdateHouseholdRequest>, UpdateHouseholdRequestValidator>();
        services.AddScoped<IValidator<CreateDailyLogRequest>, CreateDailyLogRequestValidator>();
        services.AddScoped<IValidator<CreateGoalRequest>, CreateGoalRequestValidator>();
        services.AddScoped<IValidator<UpdateGoalRequest>, UpdateGoalRequestValidator>();
        services.AddScoped<IValidator<CreateLearningPlanRequest>, CreateLearningPlanRequestValidator>();
        services.AddScoped<IValidator<UpdateLearningPlanRequest>, UpdateLearningPlanRequestValidator>();
        services.AddScoped<IValidator<CreateReadinessOutcomeRequest>, CreateReadinessOutcomeRequestValidator>();
        services.AddScoped<IValidator<UpdateReadinessOutcomeRequest>, UpdateReadinessOutcomeRequestValidator>();
        services.AddScoped<IValidator<CreateReadinessProfileRequest>, CreateReadinessProfileRequestValidator>();

        return services;
    }
}
