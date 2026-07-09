using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using EarlyLearner.Shared.Options;
using EarlyLearner.Worker.Messaging;
using EarlyLearner.Worker.Options;
using EarlyLearner.Worker.Persistence;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EarlyLearner.Worker.Configuration;

public static class WorkerAppServices
{
    public static void AddAppServices(HostApplicationBuilder builder)
    {
        builder.Services
            .AuditDatabaseServices(builder.Configuration)
            .EarlyLearnerServices(builder.Configuration)
            .AddCosmosServices(builder.Configuration)
            .EmailServices(builder.Configuration, builder.Environment)
            .MessagingServices(builder.Configuration);
    }


    private static IServiceCollection AddCosmosServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddCosmosDb(config);
        services.AddSingleton(new DocumentContainerDefinition(
            NotificationDocument.ContainerName,
            NotificationDocument.PartitionKeyPath));
        return services;
    }

    private static IServiceCollection AuditDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<AuditDatabaseOptions>()
            .Bind(configuration.GetSection(AuditDatabaseOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<AuditDbContext>((sp, options) => {
            var dbOpts = sp.GetRequiredService<IOptions<AuditDatabaseOptions>>().Value;
            options.UseNpgsql(dbOpts.AuditDb).UseSnakeCaseNamingConvention();
        });

        services.AddHostedService<AuditDbInitializer>();

        return services;
    }

    private static IServiceCollection EarlyLearnerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<EarlyLearnerOptions>()
            .Bind(configuration.GetSection(EarlyLearnerOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection EmailServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services
            .AddOptions<AzureCommunicationServiceOptions>()
            .Bind(configuration.GetSection(AzureCommunicationServiceOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .Validate(options => environment.IsDevelopment() || options.HasRequiredConfiguration(), "Azure Communication Services email requires a connection string and sender address outside development.")
            .ValidateOnStart();

        if (environment.IsDevelopment()) {
            services.AddScoped<IEmailSender, ConsoleEmailSender>();
            return services;
        }

        services.AddScoped<IEmailSender, AzureCommunicationEmailSender>();
        return services;
    }

    private static IServiceCollection MessagingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<AzureServiceBusOptions>()
            .Bind(configuration.GetSection(AzureServiceBusOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddMassTransit(configurator => {
            configurator.AddConsumersFromNamespaceContaining(typeof(ConsumerAnchor)); // register consumers in DI
            configurator.AddEntityFrameworkOutbox<AuditDbContext>(outboxConfigurator => {
                outboxConfigurator.UsePostgres();
                outboxConfigurator.UseBusOutbox();
            });

            configurator.UsingAzureServiceBus((context, busFactoryConfigurator) => {
                var options = context.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value;
                busFactoryConfigurator.DeployPublishTopology = false;

                // Register topic names per event
                busFactoryConfigurator.Message<HouseholdInvitationEmailRequestedEvent>(messageConfigurator =>
                    messageConfigurator.SetEntityName(IdentityMessagingTopology.HouseholdInvitationEmailRequestedTopic));
                busFactoryConfigurator.Message<HouseholdInvitationEmailSentEvent>(messageConfigurator =>
                    messageConfigurator.SetEntityName(IdentityMessagingTopology.HouseholdInvitationEmailSentTopic));
                busFactoryConfigurator.Message<HouseholdInvitationEmailFailedEvent>(messageConfigurator =>
                    messageConfigurator.SetEntityName(IdentityMessagingTopology.HouseholdInvitationEmailFailedTopic));
                busFactoryConfigurator.Message<AuditTrailEntryRecordedEvent>(messageConfigurator =>
                    messageConfigurator.SetEntityName(AuditMessagingTopology.AuditTrailEntryRecordedTopic));

                // Exclude the interface for integrations
                busFactoryConfigurator.Publish<IIntegrationEvent>(publishConfigurator => {
                    publishConfigurator.Exclude = true;
                });

                var administrationConnectionString = string.IsNullOrWhiteSpace(options.AdministrationConnectionString)
                    ? options.ConnectionString
                    : options.AdministrationConnectionString;

                var serviceBusClient = new ServiceBusClient(options.ConnectionString);
                var administrationClient = new ServiceBusAdministrationClient(administrationConnectionString);

                busFactoryConfigurator.Host(
                    ServiceBusConnectionStringProperties.Parse(options.ConnectionString).Endpoint,
                    serviceBusClient,
                    administrationClient);
                busFactoryConfigurator.PrefetchCount = options.PrefetchCount ?? 1;
                busFactoryConfigurator.ConcurrentMessageLimit = options.ConcurrentMessageLimit ?? 1;
                busFactoryConfigurator.UseMessageRetry(retryConfigurator => retryConfigurator.None());
                busFactoryConfigurator.UseTimeout(timeoutConfigurator => {
                    timeoutConfigurator.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds ?? 60);
                });

                // Register consumers
                busFactoryConfigurator.SubscriptionEndpoint<HouseholdInvitationEmailRequestedEvent>(
                    IdentityMessagingTopology.EmailWorkerSubscription,
                    endpointConfigurator => {
                        endpointConfigurator.ConfigureConsumeTopology = false;
                        endpointConfigurator.UseEntityFrameworkOutbox<AuditDbContext>(context);
                        endpointConfigurator.ConfigureConsumer<HouseholdInvitationEmailRequestedConsumer>(context);
                    });
                busFactoryConfigurator.SubscriptionEndpoint<AuditTrailEntryRecordedEvent>(
                    AuditMessagingTopology.AuditWorkerSubscription,
                    endpointConfigurator => {
                        endpointConfigurator.ConfigureConsumeTopology = false;
                        endpointConfigurator.UseEntityFrameworkOutbox<AuditDbContext>(context);
                        endpointConfigurator.ConfigureConsumer<AuditTrailEntryRecordedConsumer>(context);
                    });
            });
        });

        return services;
    }
}
