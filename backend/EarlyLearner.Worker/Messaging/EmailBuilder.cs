using System.Net;
using EarlyLearner.Shared.Messaging;

namespace EarlyLearner.Worker.Messaging;

public static class EmailBuilder
{
    public static EmailMessageModel BuildHouseholdInvitationEmail(HouseholdInvitationEmailRequestedEvent invitation, Uri appUrl)
    {
        var greetingName = string.IsNullOrWhiteSpace(invitation.FirstName) ? "there" : invitation.FirstName.Trim();
        var subject = $"You're invited to join {invitation.HouseholdName}";
        var expiryDate = invitation.ExpiresAt.ToString("dd MMM yyyy");

        var plainTextBody =
            $"Hi {greetingName},\n\n" +
            $"You have been invited to join {invitation.HouseholdName} on EarlyLearner.\n\n" +
            $"Open EarlyLearner to accept the invitation: {appUrl}\n\n" +
            $"This invitation expires on {expiryDate}.";

        return new EmailMessageModel(
            invitation.Email,
            subject,
            plainTextBody,
            BuildHouseholdInvitationHtml(greetingName, invitation.HouseholdName, expiryDate, appUrl));
    }

    private static string BuildHouseholdInvitationHtml(string greetingName, string householdName, string expiryDate, Uri appUrl)
    {
        var safeGreetingName = WebUtility.HtmlEncode(greetingName);
        var safeHouseholdName = WebUtility.HtmlEncode(householdName);
        var safeExpiryDate = WebUtility.HtmlEncode(expiryDate);
        var safeAppUrl = WebUtility.HtmlEncode(appUrl.ToString());

        return $$"""
            <!doctype html>
            <html lang="en">
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>EarlyLearner invitation</title>
            </head>
            <body style="margin:0;background:#f6f4f1;font-family:Arial,Helvetica,sans-serif;color:#172033;">
              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background:#f6f4f1;padding:32px 16px;">
                <tr>
                  <td align="center">
                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width:560px;background:#ffffff;border:1px solid #e8dfd7;border-radius:8px;overflow:hidden;">
                      <tr>
                        <td style="padding:28px 32px 12px 32px;">
                          <p style="margin:0 0 12px 0;color:#ef5b5b;font-size:14px;font-weight:700;">EarlyLearner</p>
                          <h1 style="margin:0;color:#101828;font-size:28px;line-height:1.25;">You're invited to join {{safeHouseholdName}}</h1>
                        </td>
                      </tr>
                      <tr>
                        <td style="padding:8px 32px 32px 32px;">
                          <p style="margin:0 0 16px 0;color:#53627a;font-size:16px;line-height:1.6;">Hi {{safeGreetingName}},</p>
                          <p style="margin:0 0 20px 0;color:#53627a;font-size:16px;line-height:1.6;">
                            You have been invited to join <strong style="color:#172033;">{{safeHouseholdName}}</strong> on EarlyLearner.
                          </p>
                          <table role="presentation" cellspacing="0" cellpadding="0" style="margin:0 0 22px 0;">
                            <tr>
                              <td style="background:#ef5b5b;border-radius:6px;">
                                <a href="{{safeAppUrl}}" style="display:inline-block;padding:12px 18px;color:#ffffff;text-decoration:none;font-size:15px;font-weight:700;">Open EarlyLearner</a>
                              </td>
                            </tr>
                          </table>
                          <p style="margin:0 0 12px 0;color:#53627a;font-size:14px;line-height:1.6;">This invitation expires on <strong>{{safeExpiryDate}}</strong>.</p>
                          <p style="margin:0;color:#53627a;font-size:12px;line-height:1.6;">If the button does not work, copy and paste this link into your browser: <br><a href="{{safeAppUrl}}" style="color:#ef5b5b;">{{safeAppUrl}}</a></p>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </body>
            </html>
            """;
    }
}
