namespace EarlyLearner.Shared.Messaging;

public static class AuditMessagingTopology
{
    public const string AuditTrailEntryRecordedTopic = "audit-trail-entry-recorded";
    public const string AuditWorkerSubscription = "earlylearner.audit-worker";
}
