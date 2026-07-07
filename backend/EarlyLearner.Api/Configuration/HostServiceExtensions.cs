using Azure.Monitor.OpenTelemetry.Exporter;
using EarlyLearner.Api.Configuration.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EarlyLearner.Api.Configuration;

/// <summary>
/// Extension methods for host-level infrastructure registrations (observability, API host configuration, middleware).
/// Kept separate from <see cref="ApiAppServices"/> so that infrastructure concerns
/// do not bleed into the application service graph.
/// </summary>
public static class HostServiceExtensions
{
    public static IHostApplicationBuilder AddEarlyLearnerObservability(this IHostApplicationBuilder builder, string serviceName)
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
}
