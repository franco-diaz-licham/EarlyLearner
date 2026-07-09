using EarlyLearner.Shared.Messaging;

namespace EarlyLearner.Shared.IdentityContext;

/// <summary>
/// Requests that an invitation email be sent to a household carer.
/// </summary>
/// <param name="Id">The unique integration event identifier.</param>
/// <param name="HouseholdId">The household associated with the invitation.</param>
/// <param name="InvitationId">The invitation to include in the email.</param>
/// <param name="HouseholdName">The display name of the household.</param>
/// <param name="Email">The recipient email address.</param>
/// <param name="FirstName">The recipient first name, when provided.</param>
/// <param name="LastName">The recipient last name, when provided.</param>
/// <param name="ExpiresAt">The date and time when the invitation expires.</param>
/// <param name="OccurredAt">The date and time when the event occurred.</param>
public sealed record HouseholdInvitationEmailRequestedEvent(
    Guid Id,
    Guid HouseholdId,
    Guid InvitationId,
    string HouseholdName,
    string Email,
    string? FirstName,
    string? LastName,
    DateTimeOffset ExpiresAt,
    DateTimeOffset OccurredAt) : IIntegrationEvent;

/// <summary>
/// Reports that a household invitation email was sent successfully.
/// </summary>
/// <param name="Id">The unique integration event identifier.</param>
/// <param name="HouseholdId">The household associated with the invitation.</param>
/// <param name="InvitationId">The invitation that was emailed.</param>
/// <param name="Email">The recipient email address.</param>
/// <param name="SentAt">The date and time when the email was sent.</param>
/// <param name="OccurredAt">The date and time when the event occurred.</param>
public sealed record HouseholdInvitationEmailSentEvent(
    Guid Id,
    Guid HouseholdId,
    Guid InvitationId,
    string Email,
    DateTimeOffset SentAt,
    DateTimeOffset OccurredAt) : IIntegrationEvent;

/// <summary>
/// Reports that a household invitation email could not be sent.
/// </summary>
/// <param name="Id">The unique integration event identifier.</param>
/// <param name="HouseholdId">The household associated with the invitation.</param>
/// <param name="InvitationId">The invitation that failed to send.</param>
/// <param name="Email">The recipient email address.</param>
/// <param name="Reason">The failure reason reported by the email sender.</param>
/// <param name="FailedAt">The date and time when the send failure was recorded.</param>
/// <param name="OccurredAt">The date and time when the event occurred.</param>
public sealed record HouseholdInvitationEmailFailedEvent(
    Guid Id,
    Guid HouseholdId,
    Guid InvitationId,
    string Email,
    string Reason,
    DateTimeOffset FailedAt,
    DateTimeOffset OccurredAt) : IIntegrationEvent;
