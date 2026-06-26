using EarlyLearner.Application.Ports;
using EarlyLearner.Shared.Options;
using EarlyLearner.Worker.Application.Ports;
using EarlyLearner.Worker.Infrastructure.AuditTrail;
using EarlyLearner.Worker.Infrastructure.Persistence;
using EarlyLearner.Worker.Messaging;
using EarlyLearner.Worker.Options;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EarlyLearner.Worker.Configuration;

public static class WorkerAppServices
{
    public static void AddAppServices(HostApplicationBuilder builder)
    {
        builder.Services
            .EarlyLearnerServices(builder.Configuration)
            .DatabaseServices(builder.Configuration)
            .EmailServices(builder.Configuration)
            .MessagingServices(builder.Configuration);
    }

    private static IServiceCollection DatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<WorkerDatabaseOptions>()
            .Bind(configuration.GetSection(WorkerDatabaseOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<WorkerDbContext>((sp, options) => {
            var databaseOptions = sp.GetRequiredService<IOptions<WorkerDatabaseOptions>>().Value;
            options.UseNpgsql(databaseOptions.Db).UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IAuditTrailWriter, AuditTrailWriter>();
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

    private static bool HasRequiredProviderConfiguration(EmailOptions options)
    {
        if (!options.UsesAzureCommunicationServices()) return true;

        return !string.IsNullOrWhiteSpace(options.AzureCommunicationConnectionString)
            && !string.IsNullOrWhiteSpace(options.SenderAddress);
    }
}
