using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.UseCases.IdentityContext;

public sealed record ExternalUserIdentity(
    string ExternalObjectId,
    string? ExternalTenantId,
    string Email,
    string FirstName,
    string LastName);

public interface ICurrentUserProvisioningService
{
    /// <summary>
    /// Creates or updates the application user for the authenticated external identity and ensures the user has a household membership.
    /// </summary>
    /// <param name="identity">The verified identity claims from the external identity provider.</param>
    /// <param name="cancellationToken">Cancels the provisioning operation.</param>
    /// <returns>The current application user model when provisioning succeeds.</returns>
    Task<Result<UserModel>> EnsureCurrentUserAsync(ExternalUserIdentity identity, CancellationToken cancellationToken);

    /// <summary>
    /// Resolves an existing application user for the authenticated external identity without creating or updating records.
    /// </summary>
    /// <param name="identity">The verified identity claims from the external identity provider.</param>
    /// <param name="cancellationToken">Cancels the lookup operation.</param>
    /// <returns>The current application user model when the user and membership already exist.</returns>
    Task<Result<UserModel>> ResolveCurrentUserAsync(ExternalUserIdentity identity, CancellationToken cancellationToken);
}

public sealed class CurrentUserProvisioningService(IUserProvisioningRepository userRepo, IUnitOfWork uow) : ICurrentUserProvisioningService
{
    public async Task<Result<UserModel>> EnsureCurrentUserAsync(ExternalUserIdentity identity, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(identity.ExternalObjectId)) return Result<UserModel>.Fail("External object id is required.", ResultTypeEnum.Unauthorized);
        if (string.IsNullOrWhiteSpace(identity.Email)) return Result<UserModel>.Fail("Email is required.", ResultTypeEnum.Unauthorized);

        var user = await userRepo.GetByExternalIdentityAsync(identity.ExternalObjectId, identity.ExternalTenantId, cancellationToken);
        user = await UpsertUser(user, identity, cancellationToken);
        if (user.Status == UserAccountStatusEnum.Disabled) return Result<UserModel>.Fail("User account is disabled.", ResultTypeEnum.Forbidden);

        await EnsureHouseholdMembership(user, identity, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        var memberships = await userRepo.GetMembershipsAsync(user.Id, cancellationToken);
        if (memberships.Count == 0) return Result<UserModel>.Fail("User household membership was not found.", ResultTypeEnum.Unauthorized);

        return Result<UserModel>.Success(Map(user, memberships), ResultTypeEnum.Success);
    }

    public async Task<Result<UserModel>> ResolveCurrentUserAsync(ExternalUserIdentity identity, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(identity.ExternalObjectId)) return Result<UserModel>.Fail("External object id is required.", ResultTypeEnum.Unauthorized);

        var user = await userRepo.GetByExternalIdentityAsync(identity.ExternalObjectId, identity.ExternalTenantId, cancellationToken);
        if (user is null) return Result<UserModel>.Fail("User was not found.", ResultTypeEnum.Unauthorized);
        if (user.Status == UserAccountStatusEnum.Disabled) return Result<UserModel>.Fail("User account is disabled.", ResultTypeEnum.Forbidden);

        var memberships = await userRepo.GetMembershipsAsync(user.Id, cancellationToken);
        if (memberships.Count == 0) return Result<UserModel>.Fail("User household membership was not found.", ResultTypeEnum.Unauthorized);

        return Result<UserModel>.Success(Map(user, memberships), ResultTypeEnum.Success);
    }

    private async Task<User> UpsertUser(User? user, ExternalUserIdentity identity, CancellationToken cancellationToken)
    {
        if (user is not null) {
            user.UpdateProfile(identity.Email, identity.FirstName, identity.LastName);
            return user;
        }

        user = await userRepo.GetByEmailAsync(identity.Email, cancellationToken);
        if (user is null) {
            user = User.CreateActiveParent(identity.Email, identity.FirstName, identity.LastName, identity.ExternalObjectId, identity.ExternalTenantId);
            userRepo.AddUser(user);
        } else {
            user.LinkExternalIdentity(identity.ExternalObjectId, identity.ExternalTenantId);
            user.UpdateProfile(identity.Email, identity.FirstName, identity.LastName);
        }

        return user;
    }

    private async Task<(HouseholdId HouseholdId, CarerId? CarerId)> EnsureHouseholdMembership(User user, ExternalUserIdentity identity, CancellationToken cancellationToken)
    {
        var membership = await userRepo.GetMembershipAsync(user.Id, cancellationToken);
        if (membership is null) {
            var householdName = $"{identity.FirstName}'s household";
            var household = Household.Create(householdName, user.Id);
            userRepo.AddHousehold(household);
            membership = (household.Id, household.Carers.First().Id);
        }

        return membership.Value;
    }

    private static UserModel Map(User user, IReadOnlyList<(HouseholdId HouseholdId, CarerId? CarerId)> memberships)
    {
        var activeMembership = memberships.First();
        var accessibleHouseholdIds = memberships.Select(membership => membership.HouseholdId).Distinct().ToArray();
        return new UserModel(user.DisplayName, user.Id, activeMembership.HouseholdId, accessibleHouseholdIds, user.Status, activeMembership.CarerId);
    }
}
