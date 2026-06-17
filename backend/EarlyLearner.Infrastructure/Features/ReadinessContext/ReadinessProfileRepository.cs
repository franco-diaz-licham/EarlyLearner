using EarlyLearner.Application.Features.ReadinessContext;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.ReadinessContext;

public sealed class ReadinessProfileRepository(DatabaseContext db) : IReadinessProfileQueryRepository, IReadinessProfileCommandRepository
{
    public async Task<List<ReadinessProfileResponse>> ListAsync(Guid householdId, CancellationToken cancellationToken)
    {
        return await db.ReadinessProfiles
            .AsNoTracking()
            .Where(profile => profile.HouseholdId.Value == householdId)
            .OrderBy(profile => profile.ChildId.Value)
            .Select(profile => new ReadinessProfileResponse(
                ReadinessProfileId: profile.Id.Value,
                HouseholdId: profile.HouseholdId.Value,
                ChildId: profile.ChildId.Value,
                ReadinessOutcomeIds: profile.OutcomeProgress.Select(progress => progress.ReadinessOutcome.Id.Value).ToList()))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ChildExistsAsync(Guid householdId, Guid childId, CancellationToken cancellationToken)
    {
        return db.Children.AnyAsync(child => child.Id.Value == childId && child.HouseholdId.Value == householdId, cancellationToken);
    }

    public async Task<List<ReadinessOutcome>> GetReadinessOutcomesAsync(IReadOnlyList<Guid> readinessOutcomeIds, CancellationToken cancellationToken)
    {
        var ids = readinessOutcomeIds.Distinct().Select(id => new ReadinessOutcomeId(id)).ToArray();
        return await db.ReadinessOutcomes
            .Where(outcome => ids.Contains(outcome.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<ReadinessProfile?> GetAsync(Guid readinessProfileId, CancellationToken cancellationToken)
    {
        return db.ReadinessProfiles.SingleOrDefaultAsync(item => item.Id.Value == readinessProfileId, cancellationToken);
    }

    public async Task<ReadinessProfileResponse?> GetResponseAsync(Guid readinessProfileId, CancellationToken cancellationToken)
    {
        return await db.ReadinessProfiles
            .AsNoTracking()
            .Where(item => item.Id.Value == readinessProfileId)
            .Select(item => new ReadinessProfileResponse(
                ReadinessProfileId: item.Id.Value,
                HouseholdId: item.HouseholdId.Value,
                ChildId: item.ChildId.Value,
                ReadinessOutcomeIds: item.OutcomeProgress.Select(progress => progress.ReadinessOutcome.Id.Value).ToList()))
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
