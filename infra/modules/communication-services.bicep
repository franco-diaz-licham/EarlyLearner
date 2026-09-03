// ------------------------------------- Parameters -------------------------------------

@description('Communication Services resource name.')
param communicationServiceName string

@description('Email Communication Service resource name.')
param emailServiceName string

@description('Azure Communication Services data residency geography.')
param dataLocation string

@description('Tags applied to Communication resources.')
param tags object

// ------------------------------------- Resources -------------------------------------

resource emailService 'Microsoft.Communication/emailServices@2025-05-01' = {
  name: emailServiceName
  location: 'global'
  tags: tags
  properties: {
    dataLocation: dataLocation
  }
}

resource azureManagedDomain 'Microsoft.Communication/emailServices/domains@2025-05-01' = {
  name: 'AzureManagedDomain'
  parent: emailService
  location: 'global'
  tags: tags
  properties: {
    domainManagement: 'AzureManaged'
    userEngagementTracking: 'Disabled'
  }
}

resource communicationService 'Microsoft.Communication/communicationServices@2025-05-01' = {
  name: communicationServiceName
  location: 'global'
  tags: tags
  properties: {
    dataLocation: dataLocation
    linkedDomains: [
      azureManagedDomain.id
    ]
    publicNetworkAccess: 'Enabled'
  }
}

// ------------------------------------- Outputs -------------------------------------

@secure()
output communicationServiceConnectionString string = communicationService.listKeys().primaryConnectionString
output communicationServiceName string = communicationService.name
output emailDomainName string = azureManagedDomain.name
output emailServiceName string = emailService.name
output mailFromSenderDomain string = azureManagedDomain.properties.mailFromSenderDomain
