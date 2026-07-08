namespace EarlyLearner.Worker.Messaging;

/// <summary>
/// Local implementation of email sender to be user for dev.
/// </summary>
public sealed class ConsoleEmailSender(ILogger<ConsoleEmailSender> logger) : IEmailSender
{
    public Task SendAsync(EmailMessageModel message, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Dev email sent to {EmailTo}. Subject: {EmailSubject}. Body: {EmailBody}",
            message.To,
            message.Subject,
            message.Body);

        return Task.CompletedTask;
    }
}
