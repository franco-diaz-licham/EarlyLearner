// ------------------------------------- Parameters -------------------------------------

@description('Azure region for Service Bus.')
param location string

@description('Service Bus namespace name.')
param namespaceName string

@description('Tags applied to Service Bus resources.')
param tags object

// ------------------------------------- Variables -------------------------------------

var topicDefaults = {
  defaultMessageTimeToLive: 'PT1H'
  duplicateDetectionHistoryTimeWindow: 'PT5M'
  requiresDuplicateDetection: true
}

var subscriptionDefaults = {
  deadLetteringOnMessageExpiration: true
  defaultMessageTimeToLive: 'PT1H'
  lockDuration: 'PT1M'
  maxDeliveryCount: 5
  requiresSession: false
}

// ------------------------------------- Resources -------------------------------------

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
  name: namespaceName
  location: location
  tags: tags
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {
    minimumTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

resource householdInvitationEmailRequestedTopic 'Microsoft.ServiceBus/namespaces/topics@2024-01-01' = {
  name: 'household-invitation-email-requested'
  parent: serviceBusNamespace
  properties: topicDefaults
}

resource householdInvitationEmailSentTopic 'Microsoft.ServiceBus/namespaces/topics@2024-01-01' = {
  name: 'household-invitation-email-sent'
  parent: serviceBusNamespace
  properties: topicDefaults
}

resource householdInvitationEmailFailedTopic 'Microsoft.ServiceBus/namespaces/topics@2024-01-01' = {
  name: 'household-invitation-email-failed'
  parent: serviceBusNamespace
  properties: topicDefaults
}

resource auditTrailEntryRecordedTopic 'Microsoft.ServiceBus/namespaces/topics@2024-01-01' = {
  name: 'audit-trail-entry-recorded'
  parent: serviceBusNamespace
  properties: topicDefaults
}

resource emailWorkerSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'earlylearner.email-worker'
  parent: householdInvitationEmailRequestedTopic
  properties: subscriptionDefaults
}

resource sentEventLogSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'earlylearner.event-log'
  parent: householdInvitationEmailSentTopic
  properties: subscriptionDefaults
}

resource sentApiNotificationSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'earlylearner.api-notifications'
  parent: householdInvitationEmailSentTopic
  properties: subscriptionDefaults
}

resource failedEventLogSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'earlylearner.event-log'
  parent: householdInvitationEmailFailedTopic
  properties: subscriptionDefaults
}

resource failedApiNotificationSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'earlylearner.api-notifications'
  parent: householdInvitationEmailFailedTopic
  properties: subscriptionDefaults
}

resource auditWorkerSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'earlylearner.audit-worker'
  parent: auditTrailEntryRecordedTopic
  properties: subscriptionDefaults
}

resource appAuthorizationRule 'Microsoft.ServiceBus/namespaces/authorizationRules@2024-01-01' = {
  name: 'EarlyLearnerApps'
  parent: serviceBusNamespace
  properties: {
    rights: [
      'Listen'
      'Send'
    ]
  }
}

// ------------------------------------- Outputs -------------------------------------

@secure()
output connectionString string = appAuthorizationRule.listKeys().primaryConnectionString
output namespaceName string = serviceBusNamespace.name
output topicNames array = [
  householdInvitationEmailRequestedTopic.name
  householdInvitationEmailSentTopic.name
  householdInvitationEmailFailedTopic.name
  auditTrailEntryRecordedTopic.name
]
