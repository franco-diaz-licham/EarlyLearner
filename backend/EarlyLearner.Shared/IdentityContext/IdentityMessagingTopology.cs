namespace EarlyLearner.Shared.IdentityContext;

public static class IdentityMessagingTopology
{
    public const string HouseholdInvitationEmailRequestedTopic = "household-invitation-email-requested";
    public const string HouseholdInvitationEmailSentTopic = "household-invitation-email-sent";
    public const string HouseholdInvitationEmailFailedTopic = "household-invitation-email-failed";

    public const string EmailWorkerSubscription = "earlylearner.email-worker";
    public const string ApiNotificationSubscription = "earlylearner.api-notifications";
}
