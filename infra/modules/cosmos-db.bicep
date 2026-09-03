// ------------------------------------- Parameters -------------------------------------

@description('Azure region for Cosmos DB.')
param location string

@description('Cosmos DB account name.')
param accountName string

@description('Cosmos DB SQL database name.')
param databaseName string

@description('Cosmos DB database shared throughput.')
@minValue(400)
param throughput int

@description('Use the one-per-subscription Cosmos DB free tier.')
param enableFreeTier bool

@description('Default TTL in seconds for short-lived document containers.')
@minValue(60)
param defaultTimeToLiveSeconds int

@description('Tags applied to Cosmos DB resources.')
param tags object

// ------------------------------------- Resources -------------------------------------

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' = {
  name: accountName
  location: location
  tags: tags
  kind: 'GlobalDocumentDB'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: false
    enableFreeTier: enableFreeTier
    locations: [
      {
        failoverPriority: 0
        isZoneRedundant: false
        locationName: location
      }
    ]
    publicNetworkAccess: 'Enabled'
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-05-15' = {
  name: databaseName
  parent: cosmosAccount
  properties: {
    options: {
      throughput: throughput
    }
    resource: {
      id: databaseName
    }
  }
}

resource notificationsContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-05-15' = {
  name: 'notifications'
  parent: database
  properties: {
    options: {}
    resource: {
      defaultTtl: defaultTimeToLiveSeconds
      id: 'notifications'
      partitionKey: {
        kind: 'Hash'
        paths: [
          '/householdId'
        ]
      }
    }
  }
}

resource notificationPublicationsContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-05-15' = {
  name: 'notification-publications'
  parent: database
  properties: {
    options: {}
    resource: {
      defaultTtl: defaultTimeToLiveSeconds
      id: 'notification-publications'
      partitionKey: {
        kind: 'Hash'
        paths: [
          '/householdId'
        ]
      }
    }
  }
}

resource userClaimsContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-05-15' = {
  name: 'user-claims'
  parent: database
  properties: {
    options: {}
    resource: {
      id: 'user-claims'
      partitionKey: {
        kind: 'Hash'
        paths: [
          '/partitionKey'
        ]
      }
    }
  }
}

// ------------------------------------- Outputs -------------------------------------

output accountName string = cosmosAccount.name
@secure()
output connectionString string = cosmosAccount.listConnectionStrings().connectionStrings[0].connectionString
output databaseName string = database.name
output containerNames array = [
  notificationsContainer.name
  notificationPublicationsContainer.name
  userClaimsContainer.name
]

