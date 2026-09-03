// ------------------------------------- Parameters -------------------------------------

@description('Azure region for SignalR Service.')
param location string

@description('SignalR Service name.')
param signalRName string

@description('Tags applied to SignalR Service.')
param tags object

// ------------------------------------- Resources -------------------------------------

resource signalR 'Microsoft.SignalRService/signalR@2024-03-01' = {
  name: signalRName
  location: location
  tags: tags
  sku: {
    capacity: 1
    name: 'Free_F1'
    tier: 'Free'
  }
  kind: 'SignalR'
  properties: {
    cors: {
      allowedOrigins: [
        '*'
      ]
    }
    features: [
      {
        flag: 'ServiceMode'
        properties: {}
        value: 'Default'
      }
    ]
    publicNetworkAccess: 'Enabled'
    tls: {
      clientCertEnabled: false
    }
  }
}

// ------------------------------------- Outputs -------------------------------------

@secure()
output connectionString string = signalR.listKeys().primaryConnectionString
output signalRName string = signalR.name
