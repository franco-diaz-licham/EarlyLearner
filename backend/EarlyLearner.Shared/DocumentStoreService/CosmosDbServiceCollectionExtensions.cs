using EarlyLearner.Shared.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EarlyLearner.Shared.DocumentStoreService;

public static class CosmosDbServiceCollectionExtensions
{
    public static IServiceCollection AddCosmosDb(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services
            .AddOptions<CosmosDbOptions>()
            .Bind(configuration.GetSection(CosmosDbOptions.SECTION_NAME))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(sp => {
            var options = sp.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
            var clientOptions = new CosmosClientOptions {
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = environment.IsDevelopment(),
                SerializerOptions = new CosmosSerializationOptions {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            };

            if (environment.IsDevelopment()) {
                clientOptions.HttpClientFactory = () => new HttpClient(new HttpClientHandler {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });
            }

            return new CosmosClient(options.ConnectionString, clientOptions);
        });

        services.AddSingleton<IDocumentStore, CosmosDocumentStore>();
        services.AddHostedService<DocumentContainerInitializer>();

        return services;
    }
}
