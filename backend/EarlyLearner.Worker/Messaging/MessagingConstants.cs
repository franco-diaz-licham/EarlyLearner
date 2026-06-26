namespace EarlyLearner.Worker.Messaging;

public static class MessagingConstants
{
    public const string EndpointPrefix = "early-learner";
    public const string HouseholdInvitationEmailRequestedEndpoint = $"{EndpointPrefix}-household-invitation-email-requested";
    public const string AuditTrailRecordRequestedEndpoint = $"{EndpointPrefix}-audit-trail-record-requested";
}
