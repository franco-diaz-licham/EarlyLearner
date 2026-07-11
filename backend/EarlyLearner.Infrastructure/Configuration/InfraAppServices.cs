using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Azure.Storage.Blobs;
using EarlyLearner.Application.UseCases.CoreContext;
using EarlyLearner.Application.UseCases.IdentityContext;
using EarlyLearner.Application.UseCases.LearningContext;
using EarlyLearner.Application.UseCases.ReadinessContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Infrastructure.Configuration.Options;
using EarlyLearner.Infrastructure.Messaging;
using EarlyLearner.Infrastructure.Persistence;
using EarlyLearner.Infrastructure.Persistence.Repositories.CoreContext;
using EarlyLearner.Infrastructure.Persistence.Repositories.IdentityContext;
using EarlyLearner.Infrastructure.Persistence.Repositories.LearningContext;
using EarlyLearner.Infrastructure.Persistence.Repositories.ReadinessContext;
using EarlyLearner.Infrastructure.Ports;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using EarlyLearner.Shared.Options;
using EarlyLearner.Shared.Realtime;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace EarlyLearner.Infrastructure.Configuration;

public static class InfraAppServices
{
    public static void AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddAzureAdAuthentication(config)
            .AddDbServices(config)
            .AddCosmosServices(config)
            .AddFileStorageServices(config)
            .AddApiMessagingServices(config)
            .AddRepositoryServices();
    }

    private static IServiceCollection AddCosmosServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddCosmosDb(config);
        services.AddSingleton(new DocumentContainerDefinition(
            NotificationDocument.ContainerName,
            NotificationDocument.PartitionKeyPath));
        return services;
    }

    public static IServiceCollection AddAzureAdAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<AzureAdOptions>()
            .Bind(configuration.GetSection(AzureAdOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(
                jwtOptions => {
                    configuration.Bind(AzureAdOptions.SECTION_NAME, jwtOptions);
                    jwtOptions.Events ??= new JwtBearerEvents();
                    var previousOnMessageReceived = jwtOptions.Events.OnMessageReceived;
                    jwtOptions.Events.OnMessageReceived = async context => {
                        if (previousOnMessageReceived is not null) await previousOnMessageReceived(context);
                        if (!string.IsNullOrWhiteSpace(context.Token)) return;
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrWhiteSpace(accessToken) && IsHubRequest(context.HttpContext.Request.Path)) context.Token = accessToken;
                    };
                },
                identityOptions => configuration.Bind(AzureAdOptions.SECTION_NAME, identityOptions));

        services.AddAuthorization(options => {
            var defaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.DefaultPolicy = defaultPolicy;
            options.FallbackPolicy = defaultPolicy;
        });

        return services;
    }

    private static bool IsHubRequest(PathString path)
    {
        return path.StartsWithSegments(RealtimeHubRoutes.VersionedHubPrefix, StringComparison.OrdinalIgnoreCase);
    }

    public static IServiceCollection AddDbServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<EncryptionOptions>()
            .Bind(configuration.GetSection(EncryptionOptions.SECTION_NAME))
            .ValidateOnStart();

        services
            .AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<DatabaseContext>((sp, options) => {
            var dbOpts = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(dbOpts.Db).UseSnakeCaseNamingConvention();
            options.ConfigureWarnings(w => {
                // Owned types are intentionally are optional. All their columns are nullable because the data may not exist
                // for every plan type. EF will return null for the owned navigation when
                // all shared columns are null, which is the desired behaviour.
                w.Ignore(RelationalEventId.OptionalDependentWithoutIdentifyingPropertyWarning);
            });
        });

        return services;
    }

    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IStoredFileQueryRepository, StoredFileRepository>();
        services.AddScoped<IStoredFileCommandRepository, StoredFileRepository>();
        services.AddScoped<IHouseholdQueryRepository, HouseholdRepository>();
        services.AddScoped<IHouseholdCommandRepository, HouseholdRepository>();
        services.AddScoped<IUserQueryRepository, UserRepository>();
        services.AddScoped<IUserProvisioningRepository, UserRepository>();
        services.AddScoped<IDailyLogQueryRepository, DailyLogRepository>();
        services.AddScoped<IDailyLogCommandRepository, DailyLogRepository>();
        services.AddScoped<IReadinessOutcomeQueryRepository, ReadinessOutcomeRepository>();
        services.AddScoped<IReadinessOutcomeCommandRepository, ReadinessOutcomeRepository>();
        services.AddScoped<IReadinessProfileQueryRepository, ReadinessProfileRepository>();
        services.AddScoped<IReadinessProfileCommandRepository, ReadinessProfileRepository>();

        return services;
    }


    public static IServiceCollection AddFileStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<AzureBlobOptions>()
            .Bind(configuration.GetSection(AzureBlobOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<BlobServiceClient>(sp => {
            var opts = sp.GetRequiredService<IOptions<AzureBlobOptions>>().Value;
            return new BlobServiceClient(opts.ConnectionString);
        });

        services.AddSingleton(sp => {
            var client = sp.GetRequiredService<BlobServiceClient>();
            var opts = sp.GetRequiredService<IOptions<AzureBlobOptions>>().Value;
            return client.GetBlobContainerClient(opts.ContainerName);
        });

        services.AddSingleton<IFileStorageService, AzureFileStorageService>();

        return services;
    }

    private static IServiceCollection AddApiMessagingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<AzureServiceBusOptions>()
            .Bind(configuration.GetSection(AzureServiceBusOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventHandler, EntityTraceAuditTrailHandler>();
        services.AddScoped<IDomainEventHandler, HouseholdCarerInvitedHandler>();
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        services.AddMassTransit(configurator => {
            configurator.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(MessagingConstants.EndpointPrefix, includeNamespace: false));
            configurator.AddConsumer<HouseholdInvitationEmailSentConsumer>();
            configurator.AddConsumer<HouseholdInvitationEmailFailedConsumer>();
            configurator.AddEntityFrameworkOutbox<DatabaseContext>(outboxConfigurator => {
                outboxConfigurator.UsePostgres();
                outboxConfigurator.UseBusOutbox();
            });

            configurator.AddConfigureEndpointsCallback((context, _, endpointConfigurator) => {
                endpointConfigurator.UseEntityFrameworkOutbox<DatabaseContext>(context);
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
                busFactoryConfigurator.SubscriptionEndpoint<HouseholdInvitationEmailSentEvent>(
                    IdentityMessagingTopology.ApiNotificationSubscription,
                    endpointConfigurator => {
                        endpointConfigurator.ConfigureConsumeTopology = false;
                        endpointConfigurator.ConfigureConsumer<HouseholdInvitationEmailSentConsumer>(context);
                    });
                busFactoryConfigurator.SubscriptionEndpoint<HouseholdInvitationEmailFailedEvent>(
                    IdentityMessagingTopology.ApiNotificationSubscription,
                    endpointConfigurator => {
                        endpointConfigurator.ConfigureConsumeTopology = false;
                        endpointConfigurator.ConfigureConsumer<HouseholdInvitationEmailFailedConsumer>(context);
                    });
            });
        });

        return services;
    }
}
