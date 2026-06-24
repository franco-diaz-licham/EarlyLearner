using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Application.Ports;

public interface IUserProvisioningRepository
{
    Task<User?> GetByExternalIdentityAsync(string externalObjectId, string? externalTenantId, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<(HouseholdId HouseholdId, CarerId? CarerId)?> GetMembershipAsync(UserId userId, CancellationToken cancellationToken);
    Task<List<(HouseholdId HouseholdId, CarerId? CarerId)>> GetMembershipsAsync(UserId userId, CancellationToken cancellationToken);
    void AddUser(User user);
    void AddHousehold(Household household);
}
