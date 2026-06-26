using EarlyLearner.Worker.Application.AuditTrail;

namespace EarlyLearner.Worker.Application.Ports;

public interface IAuditTrailWriter
{
    Task AddAsync(AuditTrailEntryModel entry, CancellationToken cancellationToken);
}
