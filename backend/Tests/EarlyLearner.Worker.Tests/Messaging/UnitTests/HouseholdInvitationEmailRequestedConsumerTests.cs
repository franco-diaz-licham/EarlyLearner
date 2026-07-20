using EarlyLearner.Shared.DocumentStoreService;
using EarlyLearner.Shared.Messaging;
using EarlyLearner.Shared.NotificationService;
using EarlyLearner.Worker.Configuration.Options;
using EarlyLearner.Worker.Messaging;
using EarlyLearner.Worker.Messaging.Consumers;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace EarlyLearner.Worker.Tests.Messaging.Consumers;

[TestFixture]
public sealed class HouseholdInvitationEmailRequestedConsumerTests
{
    private Mock<IEmailSender> _emailSender = default!;
    private Mock<IDocumentStore> _documentStore = default!;
    private HouseholdInvitationEmailRequestedConsumer _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _emailSender = new Mock<IEmailSender>(MockBehavior.Strict);
        _documentStore = new Mock<IDocumentStore>(MockBehavior.Strict);

        _sut = new HouseholdInvitationEmailRequestedConsumer(
            _emailSender.Object,
            _documentStore.Object,
            Options.Create(new EarlyLearnerOptions { Url = new Uri("https://earlylearner.test") }));
    }

    [Test]
    public async Task Consume_Should_SendEmailUpsertNotificationAndPublishSentEvent()
    {
        // Arrange
        var message = CreateRequestedEvent();
        var context = CreateContext(message);
        NotificationDocument? notification = null;
        HouseholdInvitationEmailSentEvent? publishedEvent = null;

        _emailSender
            .Setup(sender => sender.SendAsync(
                It.Is<EmailMessageModel>(email => email.To == message.Email),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _documentStore
            .Setup(store => store.UpsertAsync(
                NotificationDocument.ContainerName,
                It.IsAny<NotificationDocument>(),
                NotificationDocument.BuildPartitionKey(message.HouseholdId),
                It.IsAny<CancellationToken>()))
            .Callback<string, NotificationDocument, string, CancellationToken>((_, document, _, _) => notification = document)
            .Returns(Task.CompletedTask);
        context
            .Setup(consumeContext => consumeContext.Publish(It.IsAny<HouseholdInvitationEmailSentEvent>(), It.IsAny<CancellationToken>()))
            .Callback<HouseholdInvitationEmailSentEvent, CancellationToken>((integrationEvent, _) => publishedEvent = integrationEvent)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.Consume(context.Object);

        // Assert
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
        _documentStore.Verify(store => store.UpsertAsync(NotificationDocument.ContainerName, It.IsAny<NotificationDocument>(), NotificationDocument.BuildPartitionKey(message.HouseholdId), It.IsAny<CancellationToken>()), Times.Once);
        context.Verify(consumeContext => consumeContext.Publish(It.IsAny<HouseholdInvitationEmailSentEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        _emailSender.VerifyNoOtherCalls();
        _documentStore.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Consume_Should_UpsertNotificationAndPublishFailedEvent_WhenEmailSenderThrows()
    {
        // Arrange
        var message = CreateRequestedEvent();
        var context = CreateContext(message);
        NotificationDocument? notification = null;
        HouseholdInvitationEmailFailedEvent? publishedEvent = null;

        _emailSender
            .Setup(sender => sender.SendAsync(It.IsAny<EmailMessageModel>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Email service is unavailable."));
        _documentStore
            .Setup(store => store.UpsertAsync(
                NotificationDocument.ContainerName,
                It.IsAny<NotificationDocument>(),
                NotificationDocument.BuildPartitionKey(message.HouseholdId),
                It.IsAny<CancellationToken>()))
            .Callback<string, NotificationDocument, string, CancellationToken>((_, document, _, _) => notification = document)
            .Returns(Task.CompletedTask);
        context
            .Setup(consumeContext => consumeContext.Publish(It.IsAny<HouseholdInvitationEmailFailedEvent>(), It.IsAny<CancellationToken>()))
            .Callback<HouseholdInvitationEmailFailedEvent, CancellationToken>((integrationEvent, _) => publishedEvent = integrationEvent)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.Consume(context.Object);

        // Assert
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
        _documentStore.Verify(store => store.UpsertAsync(NotificationDocument.ContainerName, It.IsAny<NotificationDocument>(), NotificationDocument.BuildPartitionKey(message.HouseholdId), It.IsAny<CancellationToken>()), Times.Once);
        _emailSender.VerifyNoOtherCalls();
        _documentStore.VerifyNoOtherCalls();
    }

    private static Mock<ConsumeContext<HouseholdInvitationEmailRequestedEvent>> CreateContext(HouseholdInvitationEmailRequestedEvent message)
    {
        var context = new Mock<ConsumeContext<HouseholdInvitationEmailRequestedEvent>>(MockBehavior.Strict);
        context
            .SetupGet(consumeContext => consumeContext.Message)
            .Returns(message);
        context
            .SetupGet(consumeContext => consumeContext.CancellationToken)
            .Returns(CancellationToken.None);

        return context;
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