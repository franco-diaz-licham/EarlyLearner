using Azure.Storage.Blobs;
using EarlyLearner.Application.Common;
using EarlyLearner.Application.Features.AuditContext;
using EarlyLearner.Application.Features.CoreContext;
using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Features.LearningContext;
using EarlyLearner.Application.Features.ReadinessContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Infrastructure.Configuration.Options;
using EarlyLearner.Infrastructure.Features.AuditContext;
using EarlyLearner.Infrastructure.Features.CoreContext;
using EarlyLearner.Infrastructure.Features.IdentityContext;
using EarlyLearner.Infrastructure.Features.LearningContext;
using EarlyLearner.Infrastructure.Features.ReadinessContext;
using EarlyLearner.Infrastructure.Messaging;
using EarlyLearner.Infrastructure.Persistence;
using EarlyLearner.Infrastructure.Ports;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
            .AddFileStorageServices(config)
            .AddApiMessagingServices(config)
            .AddRepositoryServices();
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
                jwtOptions => configuration.Bind(AzureAdOptions.SECTION_NAME, jwtOptions),
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
        services.AddScoped<IAuditTrailQueryRepository, AuditTrailQueryRepository>();

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
        services.AddSingleton<BusObserver>();
        services
            .AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventHandler, EntityTraceAuditTrailHandler>();
        services.AddScoped<IDomainEventHandler, HouseholdCarerInvitedHandler>();
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        services.AddMassTransit(configurator => {
            configurator.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(MessagingConstants.EndpointPrefix, includeNamespace: false));
            configurator.AddEntityFrameworkOutbox<DatabaseContext>(outboxConfigurator => {
                outboxConfigurator.UsePostgres();
                outboxConfigurator.UseBusOutbox();
            });

            configurator.AddConfigureEndpointsCallback((context, _, endpointConfigurator) => {
                endpointConfigurator.UseEntityFrameworkOutbox<DatabaseContext>(context);
            });

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
                busFactoryConfigurator.ConnectBusObserver(context.GetRequiredService<BusObserver>());
                busFactoryConfigurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
