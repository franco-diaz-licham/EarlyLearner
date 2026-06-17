using EarlyLearner.Application.Features.ReadinessContext;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.ReadinessContext;

public sealed class EfReadinessProfileQueryRepository(DatabaseContext db) : IReadinessProfileQueryRepository
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
}
