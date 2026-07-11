using Azure;
using Azure.Communication.Email;
using EarlyLearner.Worker.Configuration.Options;
using Microsoft.Extensions.Options;

namespace EarlyLearner.Worker.Messaging;

public sealed class AzureCommunicationEmailSender(IOptions<AzureCommunicationServiceOptions> options, ILogger<AzureCommunicationEmailSender> logger) : IEmailSender
{
    private readonly AzureCommunicationServiceOptions options = options.Value;

    public async Task SendAsync(EmailMessageModel message, CancellationToken cancellationToken)
    {
        var emailClient = new EmailClient(options.ConnectionString);
        var emailMessage = new EmailMessage(
            senderAddress: options.SenderAddress,
            recipientAddress: message.To,
            content: new EmailContent(message.Subject) {
                PlainText = message.Body,
                Html = message.HtmlBody
            });

        var operation = await emailClient.SendAsync(WaitUntil.Completed, emailMessage, cancellationToken: cancellationToken);
        logger.LogInformation(
            "Azure Communication Services email sent to {EmailTo}. OperationId: {OperationId}. Status: {EmailStatus}",
            message.To,
            operation.Id,
            operation.Value.Status);
    }
}
