using EarlyLearner.Application.Ports;
using EarlyLearner.Shared.Options;
using EarlyLearner.Worker.Messaging;
using MassTransit;
using Microsoft.Extensions.Options;

namespace EarlyLearner.Worker.Configuration;

public static class WorkerAppServices
{
    public static void AddAppServices(HostApplicationBuilder builder)
    {
        builder.Services
            .MessagingServices(builder.Configuration);
    }

    private static IServiceCollection MessagingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailSender, ConsoleEmailSender>();

        services
            .AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddMassTransit(configurator => {
            configurator.AddConsumersFromNamespaceContaining(typeof(ConsumerAnchor));

            configurator.UsingRabbitMq((context, busFactoryConfigurator) => {
                var options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                busFactoryConfigurator.Host(new Uri(options.HostUri), hostConfigurator => {
                    hostConfigurator.Username(options.Username);
                    hostConfigurator.Password(options.Password);
                });

                busFactoryConfigurator.PrefetchCount = options.PrefetchCount ?? 1;
                busFactoryConfigurator.ConcurrentMessageLimit = options.ConcurrentMessageLimit ?? 1;
                busFactoryConfigurator.UseMessageRetry(retryConfigurator => retryConfigurator.None());
                busFactoryConfigurator.UseTimeout(timeoutConfigurator => {
                    timeoutConfigurator.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds ?? 60);
                });
                busFactoryConfigurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
