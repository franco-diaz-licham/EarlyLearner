using EarlyLearner.Api.Configuration.Options;
using EarlyLearner.Api.Endpoints;
using EarlyLearner.Application.Features.AuditContext;
using EarlyLearner.Application.Features.CoreContext;
using EarlyLearner.Application.Features.Dashboard;
using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Features.LearningContext;
using EarlyLearner.Application.Features.PlanningContext;
using EarlyLearner.Application.Features.ReadinessContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Infrastructure.Features.Dashboard;
using EarlyLearner.Infrastructure.Configuration.Options;
using FluentValidation;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EarlyLearner.Api.Configuration;

public static class ApiAppServices
{
    public static void AddAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerServices();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.ConfigureHttpJsonOptions(options => {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
        });
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddRequestValidators();

        builder.Services
            .AddUseCaseServices()
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
            options.SchemaFilter<EnumSchemaFilter>();
        });
        return services;
    }

    public static IServiceCollection AddUseCaseServices(this IServiceCollection services)
    {
        services.AddScoped<IGetHomeDashboardQueryHandler, EfGetHomeDashboardQueryHandler>();
        services.AddScoped<IStoredFileQueryService, StoredFileQueryService>();
        services.AddScoped<IStoredFileCommandService, StoredFileCommandService>();
        services.AddScoped<IHouseholdQueryService, HouseholdQueryService>();
        services.AddScoped<IHouseholdCommandService, HouseholdCommandService>();
        services.AddScoped<IUserQueryService, UserQueryService>();
        services.AddScoped<ICurrentUserProvisioningService, CurrentUserProvisioningService>();
        services.AddScoped<IDailyLogQueryService, DailyLogQueryService>();
        services.AddScoped<IDailyLogCommandService, DailyLogCommandService>();
        services.AddScoped<IGoalQueryService, GoalQueryService>();
        services.AddScoped<IGoalCommandService, GoalCommandService>();
        services.AddScoped<ILearningPlanQueryService, LearningPlanQueryService>();
        services.AddScoped<ILearningPlanCommandService, LearningPlanCommandService>();
        services.AddScoped<IReadinessOutcomeQueryService, ReadinessOutcomeQueryService>();
        services.AddScoped<IReadinessOutcomeCommandService, ReadinessOutcomeCommandService>();
        services.AddScoped<IReadinessProfileQueryService, ReadinessProfileQueryService>();
        services.AddScoped<IReadinessProfileCommandService, ReadinessProfileCommandService>();
        services.AddScoped<IAuditTrailQueryService, AuditTrailQueryService>();

        return services;
    }

    public static IServiceCollection AddPortServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
        services.AddScoped<ICachingService, MemoryCachingService>();

        return services;
    }

    private static IServiceCollection AddRequestValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateStoredFileRequest>, CreateStoredFileRequestValidator>();
        services.AddScoped<IValidator<UpdateStoredFileStatusRequest>, UpdateStoredFileStatusRequestValidator>();
        services.AddScoped<IValidator<UpdateHouseholdRequest>, UpdateHouseholdRequestValidator>();
        services.AddScoped<IValidator<InviteHouseholdCarerRequest>, InviteHouseholdCarerRequestValidator>();
        services.AddScoped<IValidator<AddHouseholdChildRequest>, AddHouseholdChildRequestValidator>();
        services.AddScoped<IValidator<UpdateHouseholdChildRequest>, UpdateHouseholdChildRequestValidator>();
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
