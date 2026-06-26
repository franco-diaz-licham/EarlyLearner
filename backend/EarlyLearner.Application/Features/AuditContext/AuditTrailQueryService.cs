using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.AuditContext;

public sealed record AuditTrailEntryResponse(
    Guid Id,
    Guid HouseholdId,
    string Action,
    string Summary,
    string? Details,
    DateTimeOffset ActionedAt,
    DateTimeOffset RecordedAt);

public interface IAuditTrailQueryService
{
    Task<Result<List<AuditTrailEntryResponse>>> ListAsync(HouseholdId householdId, string? search, CancellationToken cancellationToken);
}

public sealed class AuditTrailQueryService(IAuditTrailQueryRepository auditTrailRepo, ICurrentUser currentUser) : IAuditTrailQueryService
{
    public async Task<Result<List<AuditTrailEntryResponse>>> ListAsync(HouseholdId householdId, string? search, CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(householdId)) return Result<List<AuditTrailEntryResponse>>.Fail("Household access denied.", ResultTypeEnum.Forbidden);

        var entries = await auditTrailRepo.ListAsync(householdId, search, cancellationToken);
        return Result<List<AuditTrailEntryResponse>>.Success(entries, ResultTypeEnum.Success, entries.Count);
    }
}
