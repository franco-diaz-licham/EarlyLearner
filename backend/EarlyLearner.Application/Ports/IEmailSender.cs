namespace EarlyLearner.Application.Ports;

public sealed record EmailMessageModel(string To, string Subject, string Body, string? HtmlBody = null);

public interface IEmailSender
{
    Task SendAsync(EmailMessageModel message, CancellationToken cancellationToken);
}
