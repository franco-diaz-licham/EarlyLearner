using Azure.Monitor.OpenTelemetry.Exporter;
using EarlyLearner.Api.Configuration.Options;
using EarlyLearner.Infrastructure.Configuration.Options;
using Microsoft.OpenApi;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EarlyLearner.Api.Configuration;

/// <summary>
/// Extension methods for host-level infrastructure registrations (observability, API host configuration, middleware).
/// Kept separate from <see cref="ApiAppServices"/> so that infrastructure concerns
/// do not bleed into the application service graph.
/// </summary>
public static class ApiHostServices
{
    public static WebApplicationBuilder AddHostServices(this WebApplicationBuilder builder, string serviceName)
    {
        return builder
            .AddSerilog()
            .AddObservability(serviceName)
            .AddApiHostServices();
    }

    public static WebApplicationBuilder AddApiHostServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerServices();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.ConfigureHttpJsonOptions(options => {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
        });
        builder.Services.AddCorsPolicy(builder.Configuration);

        return builder;
    }

    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder, string serviceName)
    {
        builder.Services
            .AddOptions<ObservabilityOptions>()
            .Bind(builder.Configuration.GetSection(ObservabilityOptions.SECTION_NAME))
            .ValidateOnStart();

        var observabilityOptions = new ObservabilityOptions();
        builder.Configuration.GetSection(ObservabilityOptions.SECTION_NAME).Bind(observabilityOptions);

        var useOtlpExporter = builder.Environment.IsDevelopment() && !string.IsNullOrWhiteSpace(observabilityOptions.OtlpEndpoint);
        var useAzureMonitorExporter = !builder.Environment.IsDevelopment() && !string.IsNullOrWhiteSpace(observabilityOptions.AppInsightConnectionString);

        builder.Logging.AddOpenTelemetry(logging => {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;

            if (useOtlpExporter) logging.AddOtlpExporter(options => options.Endpoint = new Uri(observabilityOptions.OtlpEndpoint));
            if (useAzureMonitorExporter) logging.AddAzureMonitorLogExporter(options => options.ConnectionString = observabilityOptions.AppInsightConnectionString);
        });

        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing => {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("MassTransit");

                if (useOtlpExporter) tracing.AddOtlpExporter(options => options.Endpoint = new Uri(observabilityOptions.OtlpEndpoint));
                if (useAzureMonitorExporter) tracing.AddAzureMonitorTraceExporter(options => options.ConnectionString = observabilityOptions.AppInsightConnectionString);
            })
            .WithMetrics(metrics => {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (useOtlpExporter) metrics.AddOtlpExporter(options => options.Endpoint = new Uri(observabilityOptions.OtlpEndpoint));
                if (useAzureMonitorExporter) metrics.AddAzureMonitorMetricExporter(options => options.ConnectionString = observabilityOptions.AppInsightConnectionString);
            });

        return builder;
    }

    /// <summary>
    /// Configures and adds serilog as a service.
    /// </summary>
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
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

        return builder;
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
                policy.WithOrigins(corsOptions.Origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => {
            options.SwaggerDoc(name: "v1", info: new OpenApiInfo { Title = "EarlyLearner API v1" });
            options.SchemaFilter<EnumSchemaFilter>();
        });
        return services;
    }
}
