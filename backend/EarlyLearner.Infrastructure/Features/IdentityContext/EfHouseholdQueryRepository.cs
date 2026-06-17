using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.IdentityContext;

public sealed class EfHouseholdQueryRepository(DatabaseContext db) : IHouseholdQueryRepository
{
    public async Task<List<HouseholdResponse>> ListAsync(CancellationToken cancellationToken)
    {
        return await db.Households
            .AsNoTracking()
            .OrderBy(household => household.Name)
            .Select(household => new HouseholdResponse(
                HouseholdId: household.Id.Value,
                Name: household.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<HouseholdResponse?> GetResponseAsync(Guid householdId, CancellationToken cancellationToken)
    {
        return await db.Households
            .AsNoTracking()
            .Where(item => item.Id.Value == householdId)
            .Select(item => new HouseholdResponse(
                HouseholdId: item.Id.Value,
                Name: item.Name))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
