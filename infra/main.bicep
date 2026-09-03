targetScope = 'resourceGroup'

// ------------------------------------- Parameters -------------------------------------

@description('Azure region for regional resources.')
param location string

@description('Azure region for Static Web Apps. Static Web Apps Free is not available in every Azure region.')
param staticWebAppLocation string

@description('Short workload name used in Azure resource names.')
param workloadName string

@description('Environment label used in Azure resource names and tags.')
param environmentName string

@description('Tags applied to all Azure resources.')
param tags object

@description('PostgreSQL administrator login name.')
param postgresAdminLogin string

@description('PostgreSQL administrator password.')
@secure()
@minLength(12)
param postgresAdminPassword string

@description('Allow Azure-hosted services to reach PostgreSQL over its public endpoint. For stricter production networking, replace this with private networking.')
param postgresAllowAzureServices bool

@description('Additional PostgreSQL public firewall rules.')
param postgresFirewallRules array

@description('Microsoft Entra instance URL used by the API, for example https://earlylearnerapp.ciamlogin.com/.')
param azureAdInstance string

@description('Microsoft Entra tenant ID expected by the API.')
param azureAdTenantId string

@description('Microsoft Entra API app registration client ID expected by the API.')
param azureAdClientId string

@description('Frontend Microsoft Entra client ID emitted for GitHub Actions.')
param frontendEntraClientId string

@description('Frontend Microsoft Entra authority URL emitted for GitHub Actions.')
param frontendEntraAuthority string

@description('Frontend Microsoft Entra API scope emitted for GitHub Actions.')
param frontendEntraApiScope string

@description('Base64-encoded 32-byte AES-256 encryption key used outside development.')
@secure()
param encryptionKey string

@description('Azure Communication Services data residency geography.')
param communicationDataLocation string

@description('Override sender address. Leave empty to use the Azure managed email domain default DoNotReply sender.')
param communicationSenderAddress string = ''

@description('Cosmos DB database name used by document services.')
param cosmosDatabaseName string

@description('Cosmos DB database shared throughput. 400 RU/s is the minimum provisioned throughput.')
@minValue(400)
param cosmosThroughput int

@description('Use the one-per-subscription Cosmos DB free tier. Set false if the subscription already has a free-tier account.')
param cosmosEnableFreeTier bool

@description('Default TTL in seconds for short-lived document containers.')
@minValue(60)
param cosmosDefaultTimeToLiveSeconds int

@description('Blob container name used for uploaded file evidence.')
param fileBlobContainerName string

@description('Minimum API Container App replicas. Zero is cheapest and cold-starts on traffic.')
@minValue(0)
param apiMinReplicas int

@description('Maximum API Container App replicas.')
@minValue(1)
param apiMaxReplicas int

@description('Minimum worker Container App replicas. Zero is cheapest and scales from Service Bus messages.')
@minValue(0)
param workerMinReplicas int

@description('Maximum worker Container App replicas.')
@minValue(1)
param workerMaxReplicas int

// ------------------------------------- Variables -------------------------------------

var normalizedWorkloadName = toLower(replace(workloadName, '_', '-'))
var normalizedEnvironmentName = toLower(replace(environmentName, '_', '-'))
var compactName = toLower(replace(replace('${normalizedWorkloadName}${normalizedEnvironmentName}', '-', ''), '_', ''))
var resourceToken = uniqueString(resourceGroup().id, normalizedWorkloadName, normalizedEnvironmentName)
var resourcePrefix = '${normalizedWorkloadName}-${normalizedEnvironmentName}'
var sharedTags = union(tags, {
  workload: workloadName
  environment: environmentName
})

var names = {
  apiContainerApp: '${resourcePrefix}-api'
  appInsights: '${resourcePrefix}-ai'
  communicationService: take('${compactName}acs${resourceToken}', 63)
  containerAppsEnvironment: '${resourcePrefix}-cae'
  containerRegistry: take('acr${compactName}${resourceToken}', 50)
  cosmosAccount: take('${resourcePrefix}-cdb-${resourceToken}', 44)
  emailService: take('${compactName}email${resourceToken}', 63)
  keyVault: take('kv-${compactName}-${resourceToken}', 24)
  logAnalyticsWorkspace: '${resourcePrefix}-law'
  managedIdentity: '${resourcePrefix}-aca-id'
  postgresServer: take('psql-${resourcePrefix}-${resourceToken}', 63)
  serviceBusNamespace: take('${resourcePrefix}-sb-${resourceToken}', 50)
  signalR: take('${resourcePrefix}-signalr-${resourceToken}', 63)
  staticWebApp: '${resourcePrefix}-swa'
  storageAccount: take('${compactName}sa${resourceToken}', 24)
  workerContainerApp: '${resourcePrefix}-worker'
}

// ------------------------------------- Modules -------------------------------------

module frontend './modules/static-web-app.bicep' = {
  name: 'static-web-app'
  params: {
    location: staticWebAppLocation
    name: names.staticWebApp
    tags: sharedTags
  }
}

module observability './modules/observability.bicep' = {
  name: 'observability'
  params: {
    appInsightsName: names.appInsights
    containerAppsEnvironmentName: names.containerAppsEnvironment
    location: location
    logAnalyticsWorkspaceName: names.logAnalyticsWorkspace
    tags: sharedTags
  }
}

module registry './modules/container-registry.bicep' = {
  name: 'container-registry'
  params: {
    containerRegistryName: names.containerRegistry
    location: location
    managedIdentityName: names.managedIdentity
    tags: sharedTags
  }
}

module postgres './modules/postgres.bicep' = {
  name: 'postgres'
  params: {
    administratorLogin: postgresAdminLogin
    administratorLoginPassword: postgresAdminPassword
    allowAzureServices: postgresAllowAzureServices
    firewallRules: postgresFirewallRules
    location: location
    serverName: names.postgresServer
    tags: sharedTags
  }
}

module storage './modules/storage-account.bicep' = {
  name: 'storage-account'
  params: {
    blobContainerName: fileBlobContainerName
    location: location
    storageAccountName: names.storageAccount
    tags: sharedTags
  }
}

module cosmos './modules/cosmos-db.bicep' = {
  name: 'cosmos-db'
  params: {
    accountName: names.cosmosAccount
    databaseName: cosmosDatabaseName
    defaultTimeToLiveSeconds: cosmosDefaultTimeToLiveSeconds
    enableFreeTier: cosmosEnableFreeTier
    location: location
    tags: sharedTags
    throughput: cosmosThroughput
  }
}

module serviceBus './modules/service-bus.bicep' = {
  name: 'service-bus'
  params: {
    location: location
    namespaceName: names.serviceBusNamespace
    tags: sharedTags
  }
}

module signalR './modules/signalr.bicep' = {
  name: 'signalr'
  params: {
    location: location
    signalRName: names.signalR
    tags: sharedTags
  }
}

module communication './modules/communication-services.bicep' = {
  name: 'communication-services'
  params: {
    communicationServiceName: names.communicationService
    dataLocation: communicationDataLocation
    emailServiceName: names.emailService
    tags: sharedTags
  }
}

var mainDatabaseConnectionString = 'Host=${postgres.outputs.hostName};Port=5432;Database=${postgres.outputs.mainDatabaseName};Username=${postgresAdminLogin};Password=${postgresAdminPassword};Ssl Mode=Require;Trust Server Certificate=true;Maximum Pool Size=200;Timeout=60'
var auditDatabaseConnectionString = 'Host=${postgres.outputs.hostName};Port=5432;Database=${postgres.outputs.auditDatabaseName};Username=${postgresAdminLogin};Password=${postgresAdminPassword};Ssl Mode=Require;Trust Server Certificate=true;Maximum Pool Size=50;Timeout=60'
var resolvedCommunicationSenderAddress = empty(communicationSenderAddress)
  ? 'DoNotReply@${communication.outputs.mailFromSenderDomain}'
  : communicationSenderAddress

module keyVault './modules/key-vault.bicep' = {
  name: 'key-vault'
  params: {
    auditDatabaseConnectionString: auditDatabaseConnectionString
    azureBlobConnectionString: storage.outputs.connectionString
    azureCommunicationServiceConnectionString: communication.outputs.communicationServiceConnectionString
    azureServiceBusConnectionString: serviceBus.outputs.connectionString
    azureSignalRConnectionString: signalR.outputs.connectionString
    communicationSenderAddress: resolvedCommunicationSenderAddress
    cosmosDbConnectionString: cosmos.outputs.connectionString
    encryptionKey: encryptionKey
    keyVaultName: names.keyVault
    location: location
    mainDatabaseConnectionString: mainDatabaseConnectionString
    tags: sharedTags
  }
}

module api './modules/container-app.bicep' = {
  name: 'api-container-app'
  params: {
    appName: names.apiContainerApp
    containerAppsEnvironmentId: observability.outputs.containerAppsEnvironmentId
    containerRegistryLoginServer: registry.outputs.containerRegistryLoginServer
    cpu: '0.25'
    environmentVariables: [
      {
        name: 'ASPNETCORE_ENVIRONMENT'
        value: 'Production'
      }
      {
        name: 'ASPNETCORE_URLS'
        value: 'http://+:80'
      }
      {
        name: 'AzureBlob__ContainerName'
        value: fileBlobContainerName
      }
      {
        name: 'AzureAd__Instance'
        value: azureAdInstance
      }
      {
        name: 'AzureAd__TenantId'
        value: azureAdTenantId
      }
      {
        name: 'AzureAd__ClientId'
        value: azureAdClientId
      }
      {
        name: 'AzureServiceBus__PrefetchCount'
        value: '1'
      }
      {
        name: 'AzureServiceBus__ConcurrentMessageLimit'
        value: '1'
      }
      {
        name: 'AzureServiceBus__TimeoutSeconds'
        value: '60'
      }
      {
        name: 'CosmosDb__DatabaseName'
        value: cosmosDatabaseName
      }
      {
        name: 'CosmosDb__DefaultTimeToLiveSeconds'
        value: string(cosmosDefaultTimeToLiveSeconds)
      }
      {
        name: 'CosmosDb__Throughput'
        value: string(cosmosThroughput)
      }
      {
        name: 'CorsOptions__PolicyName'
        value: 'Frontend'
      }
      {
        name: 'CorsOptions__Origins__0'
        value: frontend.outputs.origin
      }
      {
        name: 'Encryption__IsEnabled'
        value: 'true'
      }
      {
        name: 'LoggingOptions__LogFilePath'
        value: 'Logs/LogFile.txt'
      }
      {
        name: 'Observability__AppInsightConnectionString'
        value: observability.outputs.applicationInsightsConnectionString
      }
    ]
    externalIngress: true
    image: 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
    location: location
    managedIdentityId: registry.outputs.managedIdentityId
    maxReplicas: apiMaxReplicas
    memory: '0.5Gi'
    minReplicas: apiMinReplicas
    scaleRules: []
    secretEnvironmentVariables: [
      {
        name: 'ConnectionStrings__Db'
        secretRef: 'main-db'
      }
      {
        name: 'AzureBlob__ConnectionString'
        secretRef: 'azure-blob'
      }
      {
        name: 'AzureServiceBus__ConnectionString'
        secretRef: 'azure-service-bus'
      }
      {
        name: 'AzureServiceBus__AdministrationConnectionString'
        secretRef: 'azure-service-bus'
      }
      {
        name: 'CosmosDb__ConnectionString'
        secretRef: 'cosmos-db'
      }
      {
        name: 'AzureSignalR__ConnectionString'
        secretRef: 'azure-signalr'
      }
      {
        name: 'Encryption__Key'
        secretRef: 'encryption-key'
      }
    ]
    secrets: {
      'azure-blob': storage.outputs.connectionString
      'azure-service-bus': serviceBus.outputs.connectionString
      'azure-signalr': signalR.outputs.connectionString
      'cosmos-db': cosmos.outputs.connectionString
      'encryption-key': encryptionKey
      'main-db': mainDatabaseConnectionString
    }
    tags: sharedTags
    targetPort: 80
  }
}

module worker './modules/container-app.bicep' = {
  name: 'worker-container-app'
  params: {
    appName: names.workerContainerApp
    containerAppsEnvironmentId: observability.outputs.containerAppsEnvironmentId
    containerRegistryLoginServer: registry.outputs.containerRegistryLoginServer
    cpu: '0.25'
    environmentVariables: [
      {
        name: 'DOTNET_ENVIRONMENT'
        value: 'Production'
      }
      {
        name: 'EarlyLearner__Url'
        value: frontend.outputs.origin
      }
      {
        name: 'AzureCommunicationService__SenderAddress'
        value: resolvedCommunicationSenderAddress
      }
      {
        name: 'AzureServiceBus__PrefetchCount'
        value: '1'
      }
      {
        name: 'AzureServiceBus__ConcurrentMessageLimit'
        value: '1'
      }
      {
        name: 'AzureServiceBus__TimeoutSeconds'
        value: '60'
      }
      {
        name: 'CosmosDb__DatabaseName'
        value: cosmosDatabaseName
      }
      {
        name: 'CosmosDb__DefaultTimeToLiveSeconds'
        value: string(cosmosDefaultTimeToLiveSeconds)
      }
      {
        name: 'CosmosDb__Throughput'
        value: string(cosmosThroughput)
      }
      {
        name: 'LoggingOptions__LogFilePath'
        value: 'Logs/LogFile.txt'
      }
      {
        name: 'Observability__AppInsightConnectionString'
        value: observability.outputs.applicationInsightsConnectionString
      }
    ]
    externalIngress: false
    image: 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
    location: location
    managedIdentityId: registry.outputs.managedIdentityId
    maxReplicas: workerMaxReplicas
    memory: '0.5Gi'
    minReplicas: workerMinReplicas
    scaleRules: [
      {
        name: 'email-worker'
        custom: {
          type: 'azure-servicebus'
          metadata: {
            messageCount: '1'
            namespace: serviceBus.outputs.namespaceName
            subscriptionName: 'earlylearner.email-worker'
            topicName: 'household-invitation-email-requested'
          }
          auth: [
            {
              secretRef: 'azure-service-bus'
              triggerParameter: 'connection'
            }
          ]
        }
      }
      {
        name: 'audit-worker'
        custom: {
          type: 'azure-servicebus'
          metadata: {
            messageCount: '1'
            namespace: serviceBus.outputs.namespaceName
            subscriptionName: 'earlylearner.audit-worker'
            topicName: 'audit-trail-entry-recorded'
          }
          auth: [
            {
              secretRef: 'azure-service-bus'
              triggerParameter: 'connection'
            }
          ]
        }
      }
    ]
    secretEnvironmentVariables: [
      {
        name: 'ConnectionStrings__AuditDb'
        secretRef: 'audit-db'
      }
      {
        name: 'AzureServiceBus__ConnectionString'
        secretRef: 'azure-service-bus'
      }
      {
        name: 'AzureServiceBus__AdministrationConnectionString'
        secretRef: 'azure-service-bus'
      }
      {
        name: 'CosmosDb__ConnectionString'
        secretRef: 'cosmos-db'
      }
      {
        name: 'AzureCommunicationService__ConnectionString'
        secretRef: 'azure-communication-service'
      }
    ]
    secrets: {
      'audit-db': auditDatabaseConnectionString
      'azure-communication-service': communication.outputs.communicationServiceConnectionString
      'azure-service-bus': serviceBus.outputs.connectionString
      'cosmos-db': cosmos.outputs.connectionString
    }
    tags: sharedTags
    targetPort: 80
  }
}

// ------------------------------------- Outputs -------------------------------------

output acrName string = registry.outputs.containerRegistryName
output apiBaseUrl string = api.outputs.defaultOrigin
output apiContainerAppName string = api.outputs.containerAppName
output applicationInsightsName string = observability.outputs.applicationInsightsName
output communicationServiceName string = communication.outputs.communicationServiceName
output cosmosDbAccountName string = cosmos.outputs.accountName
output keyVaultName string = keyVault.outputs.keyVaultName
output postgresServerName string = postgres.outputs.serverName
output resourceGroupName string = resourceGroup().name
output serviceBusNamespaceName string = serviceBus.outputs.namespaceName
output signalRName string = signalR.outputs.signalRName
output staticWebAppName string = frontend.outputs.name
output staticWebAppUrl string = frontend.outputs.origin
output storageAccountName string = storage.outputs.storageAccountName
output workerContainerAppName string = worker.outputs.containerAppName
output githubSecrets object = {
  ACR_NAME: registry.outputs.containerRegistryName
  API_BASE_URL: api.outputs.defaultOrigin
  API_CONTAINER_APP_NAME: api.outputs.containerAppName
  AZURE_RESOURCE_GROUP: resourceGroup().name
  ENTRA_API_SCOPE: frontendEntraApiScope
  ENTRA_AUTHORITY: frontendEntraAuthority
  ENTRA_CLIENT_ID: frontendEntraClientId
  RESOURCE_GROUP: resourceGroup().name
  SWA_NAME: frontend.outputs.name
  SWA_RESOURCE_GROUP: resourceGroup().name
  WORKER_CONTAINER_APP_NAME: worker.outputs.containerAppName
}

