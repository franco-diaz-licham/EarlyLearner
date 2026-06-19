using Azure.Storage.Blobs;
using EarlyLearner.Application.Common;
using EarlyLearner.Application.Features.CoreContext;
using EarlyLearner.Application.Features.Dashboard;
using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Features.LearningContext;
using EarlyLearner.Application.Features.PlanningContext;
using EarlyLearner.Application.Features.ReadinessContext;
using EarlyLearner.Infrastructure.Configuration.Options;
using EarlyLearner.Infrastructure.Features.CoreContext;
using EarlyLearner.Infrastructure.Features.Dashboard;
using EarlyLearner.Infrastructure.Features.IdentityContext;
using EarlyLearner.Infrastructure.Features.LearningContext;
using EarlyLearner.Infrastructure.Features.PlanningContext;
using EarlyLearner.Infrastructure.Features.ReadinessContext;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EarlyLearner.Infrastructure.Configuration;

public static class InfraAppServices
{
    public static void AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddDbServices(config)
            .AddFileStorageServices(config)
            .AddApplicationReadServices();
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

        // services.AddSingleton<IFileStorageService>(sp => {
        //     var containerClient = sp.GetRequiredService<BlobContainerClient>();
        //     return new AzureFileStorageService(containerClient);
        // });

        return services;
    }

    public static IServiceCollection AddApplicationReadServices(this IServiceCollection services)
    {
        services.AddScoped<IGetHomeDashboardQueryHandler, EfGetHomeDashboardQueryHandler>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IHouseholdQueryService, HouseholdQueryService>();
        services.AddScoped<IHouseholdCommandService, HouseholdCommandService>();
        services.AddScoped<HouseholdRepository>();
        services.AddScoped<IHouseholdQueryRepository>(sp => sp.GetRequiredService<HouseholdRepository>());
        services.AddScoped<IHouseholdCommandRepository>(sp => sp.GetRequiredService<HouseholdRepository>());
        services.AddScoped<IStoredFileQueryService, StoredFileQueryService>();
        services.AddScoped<IStoredFileCommandService, StoredFileCommandService>();
        services.AddScoped<StoredFileRepository>();
        services.AddScoped<IStoredFileQueryRepository>(sp => sp.GetRequiredService<StoredFileRepository>());
        services.AddScoped<IStoredFileCommandRepository>(sp => sp.GetRequiredService<StoredFileRepository>());
        services.AddScoped<IReadinessOutcomeQueryService, ReadinessOutcomeQueryService>();
        services.AddScoped<IReadinessOutcomeCommandService, ReadinessOutcomeCommandService>();
        services.AddScoped<ReadinessOutcomeRepository>();
        services.AddScoped<IReadinessOutcomeQueryRepository>(sp => sp.GetRequiredService<ReadinessOutcomeRepository>());
        services.AddScoped<IReadinessOutcomeCommandRepository>(sp => sp.GetRequiredService<ReadinessOutcomeRepository>());
        services.AddScoped<IReadinessProfileQueryService, ReadinessProfileQueryService>();
        services.AddScoped<IReadinessProfileCommandService, ReadinessProfileCommandService>();
        services.AddScoped<ReadinessProfileRepository>();
        services.AddScoped<IReadinessProfileQueryRepository>(sp => sp.GetRequiredService<ReadinessProfileRepository>());
        services.AddScoped<IReadinessProfileCommandRepository>(sp => sp.GetRequiredService<ReadinessProfileRepository>());
        services.AddScoped<IGoalQueryService, GoalQueryService>();
        services.AddScoped<IGoalCommandService, GoalCommandService>();
        services.AddScoped<GoalRepository>();
        services.AddScoped<IGoalQueryRepository>(sp => sp.GetRequiredService<GoalRepository>());
        services.AddScoped<IGoalCommandRepository>(sp => sp.GetRequiredService<GoalRepository>());
        services.AddScoped<ILearningPlanQueryService, LearningPlanQueryService>();
        services.AddScoped<ILearningPlanCommandService, LearningPlanCommandService>();
        services.AddScoped<LearningPlanRepository>();
        services.AddScoped<ILearningPlanQueryRepository>(sp => sp.GetRequiredService<LearningPlanRepository>());
        services.AddScoped<ILearningPlanCommandRepository>(sp => sp.GetRequiredService<LearningPlanRepository>());
        services.AddScoped<IDailyLogQueryService, DailyLogQueryService>();
        services.AddScoped<IDailyLogCommandService, DailyLogCommandService>();
        services.AddScoped<DailyLogRepository>();
        services.AddScoped<IDailyLogQueryRepository>(sp => sp.GetRequiredService<DailyLogRepository>());
        services.AddScoped<IDailyLogCommandRepository>(sp => sp.GetRequiredService<DailyLogRepository>());

        return services;
    }
}
