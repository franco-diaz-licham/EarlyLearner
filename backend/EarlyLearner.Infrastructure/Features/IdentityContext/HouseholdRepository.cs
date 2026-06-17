using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.IdentityContext;

public sealed class HouseholdRepository(DatabaseContext db) : IHouseholdQueryRepository, IHouseholdCommandRepository
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

    public Task<Household?> GetAsync(Guid householdId, CancellationToken cancellationToken)
    {
        return db.Households.SingleOrDefaultAsync(item => item.Id.Value == householdId, cancellationToken);
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

    public void Add(Household household)
    {
        db.Households.Add(household);
    }

    public void Remove(Household household)
    {
        db.Households.Remove(household);
    }
}
