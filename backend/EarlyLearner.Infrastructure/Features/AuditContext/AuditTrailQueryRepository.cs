using EarlyLearner.Application.Features.AuditContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.AuditContext;

public sealed class AuditTrailQueryRepository(DatabaseContext db) : IAuditTrailQueryRepository
{
    public Task<List<AuditTrailEntryResponse>> ListAsync(HouseholdId householdId, string? search, CancellationToken cancellationToken)
    {
        var query = db.AuditTrailEntries
            .AsNoTracking()
            .Where(entry => entry.HouseholdId == householdId.Value);

        if (!string.IsNullOrWhiteSpace(search)) {
            var searchTerm = search.Trim().ToLowerInvariant();
            query = query.Where(entry =>
                entry.Action.ToLower().Contains(searchTerm) ||
                entry.Summary.ToLower().Contains(searchTerm) ||
                (entry.Details != null && entry.Details.ToLower().Contains(searchTerm)));
        }

        return query
            .OrderByDescending(entry => entry.ActionedAt)
            .Select(entry => new AuditTrailEntryResponse(
                entry.Id,
                entry.HouseholdId,
                entry.Action,
                entry.Summary,
                entry.Details,
                entry.ActionedAt,
                entry.RecordedAt))
            .ToListAsync(cancellationToken);
    }
}
