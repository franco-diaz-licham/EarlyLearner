using EarlyLearner.Shared.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        // services.AddSingleton<BusObserver>();
        // services
        //     .AddOptions<RabbitMqOptions>()
        //     .Bind(configuration.GetSection(RabbitMqOptions.SECTION_NAME))
        //     .ValidateDataAnnotations()
        //     .ValidateOnStart();

        // services.AddMassTransit(configurator => {
        //     configurator.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(MessagingConstants.EndpointPrefix, includeNamespace: false));
        //     configurator.AddEntityFrameworkOutbox<DatabaseContext>(outboxConfigurator => {
        //         outboxConfigurator.UsePostgres();
        //         outboxConfigurator.UseBusOutbox();
        //     });
        //     configurator.AddConsumersFromNamespaceContaining(typeof(ConsumerAnchor));
        //     configurator.AddConfigureEndpointsCallback((context, _, endpointConfigurator) => {
        //         endpointConfigurator.UseEntityFrameworkOutbox<DatabaseContext>(context);
        //     });

        //     configurator.UsingRabbitMq((context, configurator) => {
        //         var options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
        //         configurator.Host(new Uri(options.HostUri), hostConfigurator => {
        //             hostConfigurator.Username(options.Username);
        //             hostConfigurator.Password(options.Password);
        //         });
        //         configurator.PrefetchCount = options.PrefetchCount ?? 1;                     // 1 message at a time
        //         configurator.ConcurrentMessageLimit = options.ConcurrentMessageLimit ?? 1;   // 1 concurrent process
        //         configurator.UseMessageRetry(r => r.None());                                 // no retry policy
        //         configurator.UseTimeout(timeoutConfigurator => {
        //             timeoutConfigurator.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds ?? 60);
        //         });
        //         configurator.ConnectBusObserver(context.GetRequiredService<BusObserver>());
        //         configurator.ConfigureEndpoints(context);
        //     });
        // });

        return services;
    }
}
