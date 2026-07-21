using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using EarlyLearner.Shared.Tests.Fixtures;
using EarlyLearner.Worker.Messaging;
using MassTransit;
using Moq;
using Shouldly;

namespace EarlyLearner.Worker.Tests.Messaging.Consumers;

[TestFixture]
public sealed class HouseholdInvitationEmailRequestedConsumerTests : WorkerConsumerFixture
{
    [Test]
    public async Task Consume_Should_SendEmailUpsertNotificationAndPublishSentEvent()
    {
        // Arrange
        var message = CreateRequestedEvent();
        var sut = CreateHouseholdInvitationEmailRequestedConsumer();
        var context = CreateContext(message);
        HouseholdInvitationEmailSentEvent? publishedEvent = null;

        _emailSender
            .Setup(sender => sender.SendAsync(
                It.Is<EmailMessageModel>(email => email.To == message.Email),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        context
            .Setup(consumeContext => consumeContext.Publish(It.IsAny<HouseholdInvitationEmailSentEvent>(), It.IsAny<CancellationToken>()))
            .Callback<HouseholdInvitationEmailSentEvent, CancellationToken>((integrationEvent, _) => publishedEvent = integrationEvent)
            .Returns(Task.CompletedTask);

        // Act
        await sut.Consume(context.Object);

        // Assert
        var notification = _documentStore.GetNotification(message.HouseholdId, message.InvitationId);
        notification.ShouldNotBeNull();
        notification.Id.ShouldBe(NotificationDocument.BuildId(message.InvitationId));
        notification.HouseholdId.ShouldBe(message.HouseholdId);
        notification.InvitationId.ShouldBe(message.InvitationId);
        notification.Type.ShouldBe("householdInvitationEmailSent");
        notification.Status.ShouldBe(NotificationDeliveryStatus.Succeeded);
        publishedEvent.ShouldNotBeNull();
        publishedEvent.HouseholdId.ShouldBe(message.HouseholdId);
        publishedEvent.InvitationId.ShouldBe(message.InvitationId);
        publishedEvent.Email.ShouldBe(message.Email);
        _emailSender.Verify(sender => sender.SendAsync(It.IsAny<EmailMessageModel>(), It.IsAny<CancellationToken>()), Times.Once);
        context.Verify(consumeContext => consumeContext.Publish(It.IsAny<HouseholdInvitationEmailSentEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        _emailSender.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Consume_Should_UpsertNotificationAndPublishFailedEvent_WhenEmailSenderThrows()
    {
        // Arrange
        var message = CreateRequestedEvent();
        var sut = CreateHouseholdInvitationEmailRequestedConsumer();
        var context = CreateContext(message);
        HouseholdInvitationEmailFailedEvent? publishedEvent = null;

        _emailSender
            .Setup(sender => sender.SendAsync(It.IsAny<EmailMessageModel>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Email service is unavailable."));
        context
            .Setup(consumeContext => consumeContext.Publish(It.IsAny<HouseholdInvitationEmailFailedEvent>(), It.IsAny<CancellationToken>()))
            .Callback<HouseholdInvitationEmailFailedEvent, CancellationToken>((integrationEvent, _) => publishedEvent = integrationEvent)
            .Returns(Task.CompletedTask);

        // Act
        await sut.Consume(context.Object);

        // Assert
        var notification = _documentStore.GetNotification(message.HouseholdId, message.InvitationId);
        notification.ShouldNotBeNull();
        notification.Type.ShouldBe("householdInvitationEmailFailed");
        notification.Status.ShouldBe(NotificationDeliveryStatus.Failed);
        notification.Message.ShouldContain("Email service is unavailable.");
        publishedEvent.ShouldNotBeNull();
        publishedEvent.HouseholdId.ShouldBe(message.HouseholdId);
        publishedEvent.InvitationId.ShouldBe(message.InvitationId);
        publishedEvent.Email.ShouldBe(message.Email);
        publishedEvent.Reason.ShouldBe("Email service is unavailable.");
        context.Verify(consumeContext => consumeContext.Publish(It.IsAny<HouseholdInvitationEmailFailedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        context.Verify(consumeContext => consumeContext.Publish(It.IsAny<HouseholdInvitationEmailSentEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        _emailSender.Verify(sender => sender.SendAsync(It.IsAny<EmailMessageModel>(), It.IsAny<CancellationToken>()), Times.Once);
        _emailSender.VerifyNoOtherCalls();
    }



    private static HouseholdInvitationEmailRequestedEvent CreateRequestedEvent()
    {
        return new HouseholdInvitationEmailRequestedEvent(
            Id: Guid.NewGuid(),
            HouseholdId: Guid.NewGuid(),
            InvitationId: Guid.NewGuid(),
            HouseholdName: "Early Learner Household",
            Email: "carer@example.com",
            FirstName: "Casey",
            LastName: "Carer",
            ExpiresAt: DateTimeOffset.UtcNow.AddDays(7),
            OccurredAt: DateTimeOffset.UtcNow);
    }
}


