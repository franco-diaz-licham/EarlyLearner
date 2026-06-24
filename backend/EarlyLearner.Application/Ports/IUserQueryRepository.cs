using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Application.Ports;

public interface IUserQueryRepository
{
    Task<User?> GetAsync(UserId userId, CancellationToken cancellationToken);
    Task<User?> GetByExternalObjectIdAsync(string externalObjectId, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<(HouseholdId HouseholdId, CarerId? CarerId)?> GetMembershipAsync(UserId userId, CancellationToken cancellationToken);
    Task<List<(HouseholdId HouseholdId, CarerId? CarerId)>> GetMembershipsAsync(UserId userId, CancellationToken cancellationToken);
}
