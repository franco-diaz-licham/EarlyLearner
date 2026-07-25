extern alias Api;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EarlyLearner.Application.Ports;
using EarlyLearner.Infrastructure.Persistence;
using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Tests.Fakes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Net.Mime;

namespace EarlyLearner.Shared.Tests.Fixtures;

public sealed class ApiTestApplicationFactory(string connectionString) : WebApplicationFactory<Api::Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
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

        builder.ConfigureTestServices(services => {
            services.RemoveAll<DbContextOptions<DatabaseContext>>();
            services.AddDbContext<DatabaseContext>(options => {
                options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
                options.ConfigureWarnings(warnings => {
                    warnings.Ignore(RelationalEventId.OptionalDependentWithoutIdentifyingPropertyWarning);
                });
            });

            services.RemoveAll<BlobServiceClient>();
            services.RemoveAll<BlobContainerClient>();
            services.RemoveAll<IFileStorageService>();
            services.RemoveAll<IDocumentStore>();
            services.RemoveAll<INotificationPublisher>();
            services.RemoveAll<IIntegrationEventPublisher>();
            services.RemoveAll<IHostedService>();

            services.AddSingleton(CreateBlobContainerClient());
            services.AddSingleton<IFileStorageService, NoOpFileStorageService>();
            services.AddSingleton<IDocumentStore, InMemoryDocumentStore>();
            services.AddSingleton<INotificationPublisher, InMemoryNotificationPublisher>();
            services.AddSingleton<IIntegrationEventPublisher, InMemoryIntegrationEventPublisher>();

            services
                .AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    options.DefaultScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            services.PostConfigure<AuthorizationOptions>(options => {
                var testPolicy = new AuthorizationPolicyBuilder(TestAuthHandler.SchemeName)
                    .RequireAuthenticatedUser()
                    .Build();

                options.DefaultPolicy = testPolicy;
                options.FallbackPolicy = testPolicy;
            });
        });
    }

    private static BlobContainerClient CreateBlobContainerClient()
    {
        var containerClient = new Mock<BlobContainerClient>();
        containerClient
            .Setup(client => client.CreateIfNotExistsAsync(PublicAccessType.None, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Azure.Response<BlobContainerInfo>)null!);

        return containerClient.Object;
    }

    /// <summary>
    /// File storage Fake
    /// </summary>
    private sealed class NoOpFileStorageService : IFileStorageService
    {
        public Task DeleteAsync(string key, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<Stream> DownloadAsync(string key, CancellationToken cancellationToken) => Task.FromResult<Stream>(new MemoryStream());
        public Task<string> UploadAsync(string key, ContentType contentType, Stream stream, CancellationToken cancellationToken) => Task.FromResult(key);
    }
}
