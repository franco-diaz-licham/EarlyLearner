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
                household.Id.Value,
                household.Name,
                household.Carers
                    .OrderBy(carer => carer.User.FirstName)
                    .ThenBy(carer => carer.User.LastName)
                    .Select(carer => new CarerResponse(
                        carer.Id.Value,
                        carer.UserId.Value,
                        carer.User.Email,
                        carer.User.FirstName,
                        carer.User.LastName,
                        (int)carer.Role,
                        carer.Role.ToString(),
                        carer.User.Status.ToString()))
                    .ToList(),
                household.Children
                    .Where(child => !child.IsArchived)
                    .OrderBy(child => child.FirstName)
                    .Select(child => new ChildResponse(
                        child.Id.Value,
                        child.FirstName,
                        string.Empty,
                        child.DateOfBirth))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }

    public Task<Household?> GetAsync(Guid householdId, CancellationToken cancellationToken)
    {
        return db.Households
            .Include(household => household.Carers)
            .ThenInclude(carer => carer.User)
            .Include(household => household.Children)
            .SingleOrDefaultAsync(item => item.Id.Value == householdId, cancellationToken);
    }

    public async Task<HouseholdResponse?> GetResponseAsync(Guid householdId, CancellationToken cancellationToken)
    {
        return await db.Households
            .AsNoTracking()
            .Where(item => item.Id.Value == householdId)
            .Select(item => new HouseholdResponse(
                item.Id.Value,
                item.Name,
                item.Carers
                    .OrderBy(carer => carer.User.FirstName)
                    .ThenBy(carer => carer.User.LastName)
                    .Select(carer => new CarerResponse(
                        carer.Id.Value,
                        carer.UserId.Value,
                        carer.User.Email,
                        carer.User.FirstName,
                        carer.User.LastName,
                        (int)carer.Role,
                        carer.Role.ToString(),
                        carer.User.Status.ToString()))
                    .ToList(),
                item.Children
                    .Where(child => !child.IsArchived)
                    .OrderBy(child => child.FirstName)
                    .Select(child => new ChildResponse(
                        child.Id.Value,
                        child.FirstName,
                        string.Empty,
                        child.DateOfBirth))
                    .ToList()))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return db.Users.SingleOrDefaultAsync(user => user.Id.Value == userId, cancellationToken);
    }

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return db.Users.SingleOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public void Add(Household household)
    {
        db.Households.Add(household);
    }

    public void AddUser(User user)
    {
        db.Users.Add(user);
    }

    public void Remove(Household household)
    {
        db.Households.Remove(household);
    }
}
