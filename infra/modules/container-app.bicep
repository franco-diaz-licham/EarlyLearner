// ------------------------------------- Parameters -------------------------------------

@description('Azure region for the Container App.')
param location string

@description('Container App name.')
param appName string

@description('Container Apps environment resource ID.')
param containerAppsEnvironmentId string

@description('User-assigned managed identity resource ID.')
param managedIdentityId string

@description('Container registry login server.')
param containerRegistryLoginServer string

@description('Container image. The CI/CD workflow replaces the placeholder image after the first push.')
param image string

@description('Whether the Container App should expose external ingress.')
param externalIngress bool

@description('Ingress target port. Ignored when external ingress is disabled.')
param targetPort int

@description('Minimum replicas. Use zero for the cheapest consumption shape.')
@minValue(0)
param minReplicas int

@description('Maximum replicas.')
@minValue(1)
param maxReplicas int

@description('Container CPU allocation.')
param cpu string

@description('Container memory allocation.')
param memory string

@description('Non-secret environment variables.')
param environmentVariables array

@description('Secret values exposed through secretRef environment variables.')
@secure()
param secrets object = {}

@description('Environment variables that reference Container App secrets.')
param secretEnvironmentVariables array = []

@description('Container Apps scale rules.')
param scaleRules array = []

@description('Tags applied to the Container App.')
param tags object

// ------------------------------------- Variables -------------------------------------

var secretDefinitions = [
  for secret in items(secrets): {
    name: secret.key
    value: string(secret.value)
  }
]

var ingressConfiguration = externalIngress ? {
  ingress: {
    allowInsecure: false
    external: true
    targetPort: targetPort
    transport: 'auto'
    traffic: [
      {
        latestRevision: true
        weight: 100
      }
    ]
  }
} : {}

// ------------------------------------- Resources -------------------------------------

resource containerApp 'Microsoft.App/containerApps@2026-01-01' = {
  name: appName
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}': {}
    }
  }
  properties: {
    configuration: union({
      activeRevisionsMode: 'Single'
      registries: [
        {
          identity: managedIdentityId
          server: containerRegistryLoginServer
        }
      ]
      secrets: secretDefinitions
    }, ingressConfiguration)
    managedEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'app'
          image: image
          env: concat(environmentVariables, secretEnvironmentVariables)
          resources: {
            cpu: json(cpu)
            memory: memory
          }
        }
      ]
      scale: {
        maxReplicas: maxReplicas
        minReplicas: minReplicas
        rules: scaleRules
      }
    }
  }
}

// ------------------------------------- Outputs -------------------------------------

output containerAppName string = containerApp.name
output defaultDomain string = externalIngress ? containerApp.properties.configuration.ingress.fqdn : ''
output defaultOrigin string = externalIngress ? 'https://${containerApp.properties.configuration.ingress.fqdn}' : ''
