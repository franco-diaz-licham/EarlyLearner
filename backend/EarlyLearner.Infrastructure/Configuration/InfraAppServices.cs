using Azure.Storage.Blobs;
using EarlyLearner.Infrastructure.Configuration.Options;
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
            .AddFileStorageServices(config);
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
}


