using Azure.Monitor.OpenTelemetry.Exporter;
using EarlyLearner.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EarlyLearner.Shared.Observability;

public static class ObservabilityExtensions
{
    public static IHostApplicationBuilder AddEarlyLearnerObservability(
        this IHostApplicationBuilder builder,
        IHostEnvironment environment,
        string serviceName,
        bool includeAspNetCoreInstrumentation = false)
    {
        builder.Services
            .AddOptions<ObservabilityOptions>()
            .Bind(builder.Configuration.GetSection(ObservabilityOptions.SECTION_NAME))
            .ValidateOnStart();

        var observabilityOptions = new ObservabilityOptions();
        builder.Configuration.GetSection(ObservabilityOptions.SECTION_NAME).Bind(observabilityOptions);

        var useOtlpExporter = environment.IsDevelopment() && !string.IsNullOrWhiteSpace(observabilityOptions.OtlpEndpoint);
        var useAzureMonitorExporter = !environment.IsDevelopment() && !string.IsNullOrWhiteSpace(observabilityOptions.AppInsightConnectionString);

        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing => {
                if (includeAspNetCoreInstrumentation) tracing.AddAspNetCoreInstrumentation();
                tracing
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("MassTransit");

                if (useOtlpExporter) tracing.AddOtlpExporter(options => options.Endpoint = new Uri(observabilityOptions.OtlpEndpoint));
                if (useAzureMonitorExporter) tracing.AddAzureMonitorTraceExporter(options => options.ConnectionString = observabilityOptions.AppInsightConnectionString);
            })
            .WithMetrics(metrics => {
                if (includeAspNetCoreInstrumentation) metrics.AddAspNetCoreInstrumentation();
                metrics
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
                if (useOtlpExporter) metrics.AddOtlpExporter(options => options.Endpoint = new Uri(observabilityOptions.OtlpEndpoint));
                if (useAzureMonitorExporter) metrics.AddAzureMonitorMetricExporter(options => options.ConnectionString = observabilityOptions.AppInsightConnectionString);
            });

        return builder;
    }
}
