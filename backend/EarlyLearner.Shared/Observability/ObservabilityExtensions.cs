using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EarlyLearner.Shared.Observability;

public static class ObservabilityExtensions
{
    public static IHostApplicationBuilder AddEarlyLearnerObservability(this IHostApplicationBuilder builder, string serviceName, bool includeAspNetCoreInstrumentation = false)
    {
        var otlpEndpoint = builder.Configuration.GetValue<string>("OpenTelemetry:OtlpEndpoint");

        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing => {
                if (includeAspNetCoreInstrumentation) tracing.AddAspNetCoreInstrumentation();
                tracing
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("MassTransit");

                if (!string.IsNullOrWhiteSpace(otlpEndpoint)) tracing.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
            })
            .WithMetrics(metrics => {
                if (includeAspNetCoreInstrumentation) metrics.AddAspNetCoreInstrumentation();
                metrics
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
                if (!string.IsNullOrWhiteSpace(otlpEndpoint)) metrics.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
            });

        return builder;
    }
}
