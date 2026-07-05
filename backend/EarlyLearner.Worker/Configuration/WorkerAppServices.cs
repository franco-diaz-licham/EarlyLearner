using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Shared.Options;
using EarlyLearner.Worker.Messaging;
using EarlyLearner.Worker.Options;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using MassTransit;
using Microsoft.Extensions.Options;

namespace EarlyLearner.Worker.Configuration;

public static class WorkerAppServices
{
    public static void AddAppServices(HostApplicationBuilder builder)
    {
        builder.Services
            .EarlyLearnerServices(builder.Configuration)
            .EmailServices(builder.Configuration)
            .MessagingServices(builder.Configuration);
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

    private static IServiceCollection EmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .Validate(HasRequiredProviderConfiguration, "Azure Communication Services email requires a connection string and sender address.")
            .ValidateOnStart();

        var emailProvider = configuration.GetValue<string>($"{EmailOptions.SECTION_NAME}:{nameof(EmailOptions.Provider)}");

        if (string.Equals(emailProvider, EmailProvider.AzureCommunicationServices, StringComparison.OrdinalIgnoreCase)) {
            services.AddScoped<IEmailSender, AzureCommunicationEmailSender>();
            return services;
        }

        services.AddScoped<IEmailSender, ConsoleEmailSender>();
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

            configurator.UsingAzureServiceBus((context, busFactoryConfigurator) => {
                var options = context.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value;
                busFactoryConfigurator.DeployPublishTopology = false;

                // Register topic names per event
                busFactoryConfigurator.Message<HouseholdInvitationEmailRequested>(messageConfigurator =>
                    messageConfigurator.SetEntityName(IdentityMessagingTopology.HouseholdInvitationEmailRequestedTopic));
                busFactoryConfigurator.Message<HouseholdInvitationEmailSent>(messageConfigurator =>
                    messageConfigurator.SetEntityName(IdentityMessagingTopology.HouseholdInvitationEmailSentTopic));
                busFactoryConfigurator.Message<HouseholdInvitationEmailFailed>(messageConfigurator =>
                    messageConfigurator.SetEntityName(IdentityMessagingTopology.HouseholdInvitationEmailFailedTopic));
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
                busFactoryConfigurator.SubscriptionEndpoint<HouseholdInvitationEmailRequested>(
                    IdentityMessagingTopology.EmailWorkerSubscription,
                    endpointConfigurator => {
                        endpointConfigurator.ConfigureConsumeTopology = false;
                        endpointConfigurator.ConfigureConsumer<HouseholdInvitationEmailRequestedConsumer>(context);
                    });
            });
        });

        return services;
    }

    private static bool HasRequiredProviderConfiguration(EmailOptions options)
    {
        if (!options.UsesAzureCommunicationServices()) return true;

        return !string.IsNullOrWhiteSpace(options.AzureCommunicationConnectionString)
            && !string.IsNullOrWhiteSpace(options.SenderAddress);
    }
}
