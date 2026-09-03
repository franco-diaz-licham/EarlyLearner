// ------------------------------------- Parameters -------------------------------------

@description('Azure region for Key Vault.')
param location string

@description('Key Vault name.')
param keyVaultName string

@description('Primary application database connection string.')
@secure()
param mainDatabaseConnectionString string

@description('Worker audit database connection string.')
@secure()
param auditDatabaseConnectionString string

@description('Azure Blob Storage connection string.')
@secure()
param azureBlobConnectionString string

@description('Service Bus connection string.')
@secure()
param azureServiceBusConnectionString string

@description('Cosmos DB connection string.')
@secure()
param cosmosDbConnectionString string

@description('Azure SignalR connection string.')
@secure()
param azureSignalRConnectionString string

@description('Azure Communication Services connection string.')
@secure()
param azureCommunicationServiceConnectionString string

@description('Base64-encoded encryption key.')
@secure()
param encryptionKey string

@description('ACS sender address.')
param communicationSenderAddress string

@description('Tags applied to Key Vault resources.')
param tags object

// ------------------------------------- Resources -------------------------------------

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    accessPolicies: []
    enableRbacAuthorization: true
    enableSoftDelete: true
    enabledForTemplateDeployment: true
    publicNetworkAccess: 'Enabled'
    sku: {
      family: 'A'
      name: 'standard'
    }
    softDeleteRetentionInDays: 7
    tenantId: tenant().tenantId
  }
}

resource mainDatabaseConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: 'MainDatabaseConnectionString'
  parent: keyVault
  properties: {
    value: mainDatabaseConnectionString
  }
}

resource auditDatabaseConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: 'AuditDatabaseConnectionString'
  parent: keyVault
  properties: {
    value: auditDatabaseConnectionString
  }
}

resource azureBlobConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: 'AzureBlobConnectionString'
  parent: keyVault
  properties: {
    value: azureBlobConnectionString
  }
}

resource azureServiceBusConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: 'AzureServiceBusConnectionString'
  parent: keyVault
  properties: {
    value: azureServiceBusConnectionString
  }
}

resource cosmosDbConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: 'CosmosDbConnectionString'
  parent: keyVault
  properties: {
    value: cosmosDbConnectionString
  }
}

resource azureSignalRConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: 'AzureSignalRConnectionString'
  parent: keyVault
  properties: {
    value: azureSignalRConnectionString
  }
}

resource azureCommunicationServiceConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: 'AzureCommunicationServiceConnectionString'
  parent: keyVault
  properties: {
    value: azureCommunicationServiceConnectionString
  }
}

resource encryptionKeySecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: 'EncryptionKey'
  parent: keyVault
  properties: {
    value: encryptionKey
  }
}

resource communicationSenderAddressSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: 'AzureCommunicationServiceSenderAddress'
  parent: keyVault
  properties: {
    value: communicationSenderAddress
  }
}

// ------------------------------------- Outputs -------------------------------------

output keyVaultName string = keyVault.name
output mainDatabaseConnectionSecretUri string = mainDatabaseConnectionSecret.properties.secretUri
output auditDatabaseConnectionSecretUri string = auditDatabaseConnectionSecret.properties.secretUri
output azureBlobConnectionSecretUri string = azureBlobConnectionSecret.properties.secretUri
output azureServiceBusConnectionSecretUri string = azureServiceBusConnectionSecret.properties.secretUri
output cosmosDbConnectionSecretUri string = cosmosDbConnectionSecret.properties.secretUri
output azureSignalRConnectionSecretUri string = azureSignalRConnectionSecret.properties.secretUri
output azureCommunicationServiceConnectionSecretUri string = azureCommunicationServiceConnectionSecret.properties.secretUri
output encryptionKeySecretUri string = encryptionKeySecret.properties.secretUri
output communicationSenderAddressSecretUri string = communicationSenderAddressSecret.properties.secretUri
