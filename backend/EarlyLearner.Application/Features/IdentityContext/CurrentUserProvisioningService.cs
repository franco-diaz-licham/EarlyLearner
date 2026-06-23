using EarlyLearner.Application.Common;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.IdentityContext;

public sealed record ExternalUserIdentity(
    string ExternalObjectId,
    string? ExternalTenantId,
    string Email,
    string FirstName,
    string LastName);

public interface ICurrentUserProvisioningService
{
    Task<Result<UserModel>> ResolveCurrentUserAsync(ExternalUserIdentity identity, CancellationToken cancellationToken);
}

public sealed class CurrentUserProvisioningService(IUserProvisioningRepository userRepo, IUnitOfWork uow) : ICurrentUserProvisioningService
{
    public async Task<Result<UserModel>> ResolveCurrentUserAsync(ExternalUserIdentity identity, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(identity.ExternalObjectId)) return Result<UserModel>.Fail("External object id is required.", ResultTypeEnum.Unauthorized);
        if (string.IsNullOrWhiteSpace(identity.Email)) return Result<UserModel>.Fail("Email is required.", ResultTypeEnum.Unauthorized);

        var user = await userRepo.GetByExternalIdentityAsync(identity.ExternalObjectId, identity.ExternalTenantId, cancellationToken);
        await UpsertUser(user, identity, cancellationToken);
        if (user!.Status == UserAccountStatusEnum.Disabled) return Result<UserModel>.Fail("User account is disabled.", ResultTypeEnum.Forbidden);

        var membership = await userRepo.GetMembershipAsync(user.Id, cancellationToken);
        if (membership is null) {
            var householdName = $"{identity.FirstName}'s household";
            var household = Household.Create(householdName, user.Id, identity.FirstName, identity.LastName);
            userRepo.AddHousehold(household);
            membership = (household.Id, null);
        }

        await uow.SaveChangesAsync(cancellationToken);
        return Result<UserModel>.Success(Map(user, membership.Value.HouseholdId, membership.Value.CarerId), ResultTypeEnum.Success);
    }

    private async Task UpsertUser(User? user, ExternalUserIdentity identity, CancellationToken cancellationToken)
    {
        if (user is null) {
            user = await userRepo.GetByEmailAsync(identity.Email, cancellationToken);
            if (user is null) {
                user = User.CreateActiveParent(identity.Email, identity.FirstName, identity.LastName, identity.ExternalObjectId, identity.ExternalTenantId);
                userRepo.AddUser(user);
            } else {
                user.LinkExternalIdentity(identity.ExternalObjectId, identity.ExternalTenantId);
                user.UpdateProfile(identity.Email, identity.FirstName, identity.LastName);
            }
        } else {
            user.UpdateProfile(identity.Email, identity.FirstName, identity.LastName);
        }
    }

    private static UserModel Map(User user, HouseholdId householdId, CarerId? carerId)
    {
        return new UserModel(user.DisplayName, user.Id, householdId, false, user.Status, carerId);
    }
}
