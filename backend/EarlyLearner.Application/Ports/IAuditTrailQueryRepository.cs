using EarlyLearner.Application.Features.AuditContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Application.Ports;

public interface IAuditTrailQueryRepository
{
    Task<List<AuditTrailEntryResponse>> ListAsync(HouseholdId householdId, string? search, CancellationToken cancellationToken);
}
