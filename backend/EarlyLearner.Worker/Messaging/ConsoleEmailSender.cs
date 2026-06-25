using EarlyLearner.Application.Ports;

namespace EarlyLearner.Worker.Messaging;

public sealed class ConsoleEmailSender(ILogger<ConsoleEmailSender> logger) : IEmailSender
{
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Dev email sent to {EmailTo}. Subject: {EmailSubject}. Body: {EmailBody}",
            message.To,
            message.Subject,
            message.Body);

        return Task.CompletedTask;
    }
}
