using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.IdentityContext;

public interface IUserQueryService
{
    Task<Result<UserModel>> GetCurrentUserAsync(UserId id, CancellationToken cancellationToken);
    Task<Result<UserModel>> GetUserByObjectIdAsync(string objectId, CancellationToken cancellationToken = default);
    Task<Result<UserModel>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}

public sealed class UserQueryService(IUserQueryRepository userQueryRepo, ICurrentUser currentUser) : IUserQueryService
{
    public async Task<Result<UserModel>> GetCurrentUserAsync(UserId id, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated) return Result<UserModel>.Fail("Current user is not authenticated.", ResultTypeEnum.Unauthorized);
        if (currentUser.UserId != id) return Result<UserModel>.Fail("Current user does not match the requested user.", ResultTypeEnum.Forbidden);

        var user = await userQueryRepo.GetAsync(id, cancellationToken);
        return user is null
            ? Result<UserModel>.Fail("User was not found.", ResultTypeEnum.NotFound)
            : Result<UserModel>.Success(Map(user, currentUser.HouseholdId, currentUser.CarerId), ResultTypeEnum.Success);
    }


    public async Task<Result<UserModel>> GetUserByObjectIdAsync(string objectId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objectId)) return Result<UserModel>.Fail("External object id is required.", ResultTypeEnum.Invalid);

        var user = await userQueryRepo.GetByExternalObjectIdAsync(objectId, cancellationToken);
        return await ToResultAsync(user, cancellationToken);
    }

    public async Task<Result<UserModel>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return Result<UserModel>.Fail("Email is required.", ResultTypeEnum.Invalid);

        var user = await userQueryRepo.GetByEmailAsync(email, cancellationToken);
        return await ToResultAsync(user, cancellationToken);
    }

    private async Task<Result<UserModel>> ToResultAsync(User? user, CancellationToken cancellationToken)
    {
        if (user is null) return Result<UserModel>.Fail("User was not found.", ResultTypeEnum.NotFound);

        var membership = await userQueryRepo.GetMembershipAsync(user.Id, cancellationToken);
        if (membership is null) return Result<UserModel>.Fail("User does not belong to a household.", ResultTypeEnum.NotFound);

        return Result<UserModel>.Success(Map(user, membership.Value.HouseholdId, membership.Value.CarerId), ResultTypeEnum.Success);
    }

    private static UserModel Map(User user, HouseholdId householdId, CarerId? carerId)
    {
        return new UserModel(user.DisplayName, user.Id, householdId, false, user.Status, carerId);
    }
}
