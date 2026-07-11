using Azure.Monitor.OpenTelemetry.Exporter;
using EarlyLearner.Worker.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EarlyLearner.Worker.Configuration;

/// <summary>
/// Extension methods for host-level infrastructure registrations (observability, worker host configuration).
/// Kept separate from <see cref="WorkerAppServices"/> so that infrastructure concerns
/// do not bleed into the application service graph.
/// </summary>
public static class WorkerHostServices
{
    public static IHostApplicationBuilder AddHostServices(this IHostApplicationBuilder builder, string serviceName)
    {
        return builder.AddObservability(serviceName);
    }

    public static IHostApplicationBuilder AddObservability(this IHostApplicationBuilder builder, string serviceName)
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
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("MassTransit");

                if (useOtlpExporter) tracing.AddOtlpExporter(options => options.Endpoint = new Uri(observabilityOptions.OtlpEndpoint));
                if (useAzureMonitorExporter) tracing.AddAzureMonitorTraceExporter(options => options.ConnectionString = observabilityOptions.AppInsightConnectionString);
            })
            .WithMetrics(metrics => {
                metrics
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (useOtlpExporter) metrics.AddOtlpExporter(options => options.Endpoint = new Uri(observabilityOptions.OtlpEndpoint));
                if (useAzureMonitorExporter) metrics.AddAzureMonitorMetricExporter(options => options.ConnectionString = observabilityOptions.AppInsightConnectionString);
            });

        return builder;
    }
}
