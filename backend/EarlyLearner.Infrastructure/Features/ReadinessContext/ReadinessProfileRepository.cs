using EarlyLearner.Application.UseCases.ReadinessContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.ReadinessContext;

public sealed class ReadinessProfileRepository(DatabaseContext db) : IReadinessProfileQueryRepository, IReadinessProfileCommandRepository
{
    public async Task<List<ReadinessProfileResponse>> ListAsync(HouseholdId householdId, CancellationToken cancellationToken)
    {
        return await db.ReadinessProfiles
            .AsNoTracking()
            .Where(profile => profile.HouseholdId == householdId)
            .OrderBy(profile => profile.ChildId.Value)
            .Select(profile => new ReadinessProfileResponse(
                ReadinessProfileId: profile.Id.Value,
                HouseholdId: profile.HouseholdId.Value,
                ChildId: profile.ChildId.Value,
                ReadinessOutcomeIds: profile.TrackedOutcomes.Select(trackedOutcome => trackedOutcome.ReadinessOutcome.Id.Value).ToList()))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ChildExistsAsync(HouseholdId householdId, ChildId childId, CancellationToken cancellationToken)
    {
        return db.Children.AnyAsync(child => child.Id == childId && child.HouseholdId == householdId, cancellationToken);
    }

    public async Task<List<ReadinessOutcome>> GetReadinessOutcomesAsync(IReadOnlyList<ReadinessOutcomeId> readinessOutcomeIds, CancellationToken cancellationToken)
    {
        var ids = readinessOutcomeIds.Distinct().ToArray();
        return await db.ReadinessOutcomes
            .Where(outcome => ids.Contains(outcome.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<ReadinessProfile?> GetAsync(ReadinessProfileId readinessProfileId, CancellationToken cancellationToken)
    {
        return db.ReadinessProfiles.SingleOrDefaultAsync(item => item.Id == readinessProfileId, cancellationToken);
    }

    public async Task<ReadinessProfileResponse?> GetResponseAsync(ReadinessProfileId readinessProfileId, CancellationToken cancellationToken)
    {
        return await db.ReadinessProfiles
            .AsNoTracking()
            .Where(item => item.Id == readinessProfileId)
            .Select(item => new ReadinessProfileResponse(
                ReadinessProfileId: item.Id.Value,
                HouseholdId: item.HouseholdId.Value,
                ChildId: item.ChildId.Value,
                ReadinessOutcomeIds: item.TrackedOutcomes.Select(trackedOutcome => trackedOutcome.ReadinessOutcome.Id.Value).ToList()))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public void Add(ReadinessProfile readinessProfile)
    {
        db.ReadinessProfiles.Add(readinessProfile);
    }

    public void Remove(ReadinessProfile readinessProfile)
    {
        db.ReadinessProfiles.Remove(readinessProfile);
    }
}
