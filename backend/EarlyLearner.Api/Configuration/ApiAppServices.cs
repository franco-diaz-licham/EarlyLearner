using EarlyLearner.Api.Configuration.Options;
using EarlyLearner.Api.Endpoints;
using EarlyLearner.Api.Notifications;
using EarlyLearner.Application.UseCases.CoreContext;
using EarlyLearner.Application.UseCases.Dashboard;
using EarlyLearner.Application.UseCases.IdentityContext;
using EarlyLearner.Application.UseCases.LearningContext;
using EarlyLearner.Application.UseCases.Notifications;
using EarlyLearner.Application.UseCases.ReadinessContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Infrastructure.Features.Dashboard;
using EarlyLearner.Shared.DocumentStoreService;
using FluentValidation;

namespace EarlyLearner.Api.Configuration;

public static class ApiAppServices
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHttpContextAccessor()
            .AddRequestValidators()
            .AddUseCaseServices()
            .AddPortServices()
            .AddNotifications(configuration);

        return services;
    }

    private static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<AzureSignalROptions>()
            .Bind(configuration.GetSection(AzureSignalROptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var signalR = services.AddSignalR();
        var azureSignalROptions = configuration
            .GetSection(AzureSignalROptions.SECTION_NAME)
            .Get<AzureSignalROptions>() ?? new AzureSignalROptions();

        if (!string.IsNullOrWhiteSpace(azureSignalROptions.ConnectionString)) signalR.AddAzureSignalR(azureSignalROptions.ConnectionString);

        services.AddSingleton<INotificationPublisher, NotificationHubPublisher>();

        return services;
    }

    public static IServiceCollection AddUseCaseServices(this IServiceCollection services)
    {
        services.AddScoped<IGetHomeDashboardQueryHandler, EfGetHomeDashboardQueryHandler>();
        services.AddScoped<IStoredFileQueryService, StoredFileQueryService>();
        services.AddScoped<IStoredFileCommandService, StoredFileCommandService>();
        services.AddScoped<IHouseholdQueryService, HouseholdQueryService>();
        services.AddScoped<IHouseholdCommandService, HouseholdCommandService>();
        services.AddScoped<IUserQueryService, UserQueryService>();
        services.AddScoped<ICurrentUserProvisioningService, CurrentUserProvisioningService>();
        services.AddScoped<IDailyLogQueryService, DailyLogQueryService>();
        services.AddScoped<IDailyLogCommandService, DailyLogCommandService>();
        services.AddScoped<IReadinessOutcomeQueryService, ReadinessOutcomeQueryService>();
        services.AddScoped<IReadinessOutcomeCommandService, ReadinessOutcomeCommandService>();
        services.AddScoped<IReadinessProfileQueryService, ReadinessProfileQueryService>();
        services.AddScoped<IReadinessProfileCommandService, ReadinessProfileCommandService>();

        return services;
    }

    public static IServiceCollection AddPortServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
        services.AddScoped<IUserClaimsCache, UserClaimsCache>();
        services.AddSingleton(new DocumentContainerDefinition(
            UserClaimsCache.ClaimsContainerName,
            UserClaimsCache.ClaimsPartitionKeyPath,
            -1));

        return services;
    }

    private static IServiceCollection AddRequestValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<StoredFileRequest>, StoredFileRequestValidator>();
        services.AddScoped<IValidator<CreateStoredFileRequest>, CreateStoredFileRequestValidator>();
        services.AddScoped<IValidator<UpdateStoredFileStatusRequest>, UpdateStoredFileStatusRequestValidator>();
        services.AddScoped<IValidator<UpdateHouseholdRequest>, UpdateHouseholdRequestValidator>();
        services.AddScoped<IValidator<InviteHouseholdCarerRequest>, InviteHouseholdCarerRequestValidator>();
        services.AddScoped<IValidator<AddHouseholdChildRequest>, AddHouseholdChildRequestValidator>();
        services.AddScoped<IValidator<UpdateHouseholdChildRequest>, UpdateHouseholdChildRequestValidator>();
        services.AddScoped<IValidator<CreateDailyLogRequest>, CreateDailyLogRequestValidator>();
        services.AddScoped<IValidator<CreateReadinessOutcomeRequest>, CreateReadinessOutcomeRequestValidator>();
        services.AddScoped<IValidator<UpdateReadinessOutcomeRequest>, UpdateReadinessOutcomeRequestValidator>();
        services.AddScoped<IValidator<CreateReadinessProfileRequest>, CreateReadinessProfileRequestValidator>();

        return services;
    }
}
