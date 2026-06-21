using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.IdentityContext;

public class UserRepository(DatabaseContext db) : IUserQueryRepository
{
    public async Task<User?> GetAsync(UserId userId, CancellationToken cancellationToken)
    {
        return await db.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public Task<User?> GetByExternalObjectIdAsync(string externalObjectId, CancellationToken cancellationToken)
    {
        var normalizedObjectId = externalObjectId.Trim();
        return db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.ExternalObjectId == normalizedObjectId, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task<(HouseholdId HouseholdId, CarerId? CarerId)?> GetMembershipAsync(UserId userId, CancellationToken cancellationToken)
    {
        var membership = await db.Carers
            .AsNoTracking()
            .Where(carer => carer.UserId == userId)
            .OrderBy(carer => carer.CreatedOn)
            .Select(carer => new { carer.HouseholdId, CarerId = (CarerId?)carer.Id })
            .FirstOrDefaultAsync(cancellationToken);

        return membership is null ? null : (membership.HouseholdId, membership.CarerId);
    }
}
