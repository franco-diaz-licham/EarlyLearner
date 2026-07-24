extern alias Api;

using EarlyLearner.Api.Tests.Fixtures;
using EarlyLearner.Application.Ports;
using EarlyLearner.Infrastructure.Persistence;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Tests.Fakes;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Net.Mime;

namespace EarlyLearner.Api.Tests.Fixtures;

public sealed class ApiTestApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration(configuration => {
            configuration.AddInMemoryCollection(new Dictionary<string, string?> {
                ["AzureAd:Instance"] = "https://login.microsoftonline.com/",
                ["AzureAd:TenantId"] = "test-tenant-id",
                ["AzureAd:ClientId"] = "test-client-id",
                ["AzureBlob:ConnectionString"] = "UseDevelopmentStorage=true",
                ["AzureBlob:ContainerName"] = "stored-files",
                ["AzureSignalR:ConnectionString"] = string.Empty,
                ["AzureServiceBus:ConnectionString"] = "Endpoint=sb://earlylearner-test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test",
                ["ConnectionStrings:Db"] = connectionString,
                ["Cors:PolicyName"] = "TestCors",
                ["Cors:Origins:0"] = "http://localhost",
                ["CosmosDb:ConnectionString"] = "AccountEndpoint=https://localhost:8081/;AccountKey=test;",
                ["CosmosDb:DatabaseName"] = "earlylearner-tests",
                ["Serilog:LogFilePath"] = "Logs/test-api.log"
            });
        });

        builder.ConfigureServices(services => {
            services.RemoveAll<DbContextOptions<DatabaseContext>>();
            services.AddDbContext<DatabaseContext>(options => {
                options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
                options.ConfigureWarnings(warnings => {
                    warnings.Ignore(RelationalEventId.OptionalDependentWithoutIdentifyingPropertyWarning);
                });
            });

                services.RemoveAll<IFileStorageService>();
                services.RemoveAll<IDocumentStore>();
                services.RemoveAll<INotificationPublisher>();
                services.RemoveAll<IIntegrationEventPublisher>();
                services.RemoveAll<IHostedService>();
                services.RemoveAll<IBus>();
                services.RemoveAll<IBusControl>();

                services.AddSingleton<IDocumentStore, InMemoryDocumentStore>();
                services.AddSingleton<INotificationPublisher, InMemoryNotificationPublisher>();
                services.AddSingleton<IIntegrationEventPublisher, InMemoryIntegrationEventPublisher>();
                services.AddSingleton<IFileStorageService, NoOpFileStorageService>();

            services
                .AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    options.DefaultScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }

    private sealed class NoOpFileStorageService : IFileStorageService
    {
        public Task DeleteAsync(string key, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<Stream> DownloadAsync(string key, CancellationToken cancellationToken)
        {
            return Task.FromResult<Stream>(new MemoryStream());
        }

        public Task<string> UploadAsync(string key, ContentType contentType, Stream stream, CancellationToken cancellationToken)
        {
            return Task.FromResult(key);
        }
    }
}
