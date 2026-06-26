using EarlyLearner.Application.Features.AuditContext;
using EarlyLearner.Worker.Application.AuditTrail;
using EarlyLearner.Worker.Application.Ports;
using MassTransit;

namespace EarlyLearner.Worker.Messaging;

public sealed class AuditTrailRecordRequestedConsumerDefinition : ConsumerDefinition<AuditTrailRecordRequestedConsumer>
{
    public AuditTrailRecordRequestedConsumerDefinition()
    {
        EndpointName = MessagingConstants.AuditTrailRecordRequestedEndpoint;
    }
}

public sealed class AuditTrailRecordRequestedConsumer(IAuditTrailWriter auditTrailWriter) : IConsumer<AuditTrailRecordRequested>
{
    public async Task Consume(ConsumeContext<AuditTrailRecordRequested> context)
    {
        var message = context.Message;

        await auditTrailWriter.AddAsync(new AuditTrailEntryModel(
            Id: message.Id,
            HouseholdId: message.HouseholdId,
            Action: message.Action,
            Summary: message.Summary,
            Details: message.Details,
            ActionedAt: message.ActionedAt,
            RecordedAt: DateTimeOffset.UtcNow), context.CancellationToken);
    }
}
