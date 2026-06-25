using EarlyLearner.Application.Common;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;
using EarlyLearner.Application.Ports;

namespace EarlyLearner.Application.Features.IdentityContext;

public sealed record UpdateHouseholdCommand(Guid HouseholdId, string Name);

public sealed record AddHouseholdCarerCommand(Guid HouseholdId, string Email, HouseholdRoleEnum Role);

public sealed record RemoveHouseholdCarerCommand(Guid HouseholdId, Guid CarerId);

public sealed record AddHouseholdChildCommand(Guid HouseholdId, string FirstName, string LastName, DateOnly DateOfBirth);

public sealed record UpdateHouseholdChildCommand(Guid HouseholdId, Guid ChildId, string FirstName, string LastName, DateOnly DateOfBirth);

public sealed record RemoveHouseholdChildCommand(Guid HouseholdId, Guid ChildId);

public interface IHouseholdCommandService
{
    Task<Result<HouseholdResponse>> UpdateAsync(UpdateHouseholdCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> AddCarerAsync(AddHouseholdCarerCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> RemoveCarerAsync(RemoveHouseholdCarerCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> AddChildAsync(AddHouseholdChildCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> UpdateChildAsync(UpdateHouseholdChildCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> RemoveChildAsync(RemoveHouseholdChildCommand command, CancellationToken cancellationToken);
}

public interface IHouseholdCommandRepository
{
    Task<Household?> GetAsync(HouseholdId householdId, CancellationToken cancellationToken);
    Task<HouseholdResponse?> GetResponseAsync(HouseholdId householdId, CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    void AddUser(User user);
}

public sealed class HouseholdCommandService(IHouseholdCommandRepository householdRepo, IUnitOfWork uow, ICurrentUser user) : IHouseholdCommandService
{
    private static readonly TimeSpan InvitationLifetime = TimeSpan.FromDays(1);

    public async Task<Result<HouseholdResponse>> UpdateAsync(UpdateHouseholdCommand command, CancellationToken cancellationToken)
    {
        if (!CanAccess(command.HouseholdId)) return Result<HouseholdResponse>.Fail("Household access denied.", ResultTypeEnum.Forbidden);

        var household = await householdRepo.GetAsync(new HouseholdId(command.HouseholdId), cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        household.Rename(command.Name);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Household could not be updated.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after update.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result<HouseholdResponse>> AddCarerAsync(AddHouseholdCarerCommand command, CancellationToken cancellationToken)
    {
        if (!CanAccess(command.HouseholdId)) return Result<HouseholdResponse>.Fail("Household access denied.", ResultTypeEnum.Forbidden);

        var household = await householdRepo.GetAsync(new HouseholdId(command.HouseholdId), cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        var invitedUser = await householdRepo.GetUserByEmailAsync(command.Email, cancellationToken);
        var expiresAt = DateTimeOffset.UtcNow.Add(InvitationLifetime);
        if (invitedUser is null) household.InviteNewCarer(command.Email, command.Role, user.UserId, expiresAt);
        else household.InviteExistingCarer(command.Email, command.Role, user.UserId, expiresAt);

        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Carer invitation could not be saved.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after invitation.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result<HouseholdResponse>> RemoveCarerAsync(RemoveHouseholdCarerCommand command, CancellationToken cancellationToken)
    {
        if (!CanAccess(command.HouseholdId)) return Result<HouseholdResponse>.Fail("Household access denied.", ResultTypeEnum.Forbidden);

        var household = await householdRepo.GetAsync(new HouseholdId(command.HouseholdId), cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        household.RemoveCarer(new CarerId(command.CarerId));
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Carer could not be removed.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after removing carer.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result<HouseholdResponse>> AddChildAsync(AddHouseholdChildCommand command, CancellationToken cancellationToken)
    {
        if (!CanAccess(command.HouseholdId)) return Result<HouseholdResponse>.Fail("Household access denied.", ResultTypeEnum.Forbidden);

        var household = await householdRepo.GetAsync(new HouseholdId(command.HouseholdId), cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        household.AddChild(command.FirstName, command.LastName, command.DateOfBirth);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Child could not be added.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after adding child.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result<HouseholdResponse>> RemoveChildAsync(RemoveHouseholdChildCommand command, CancellationToken cancellationToken)
    {
        if (!CanAccess(command.HouseholdId)) return Result<HouseholdResponse>.Fail("Household access denied.", ResultTypeEnum.Forbidden);

        var household = await householdRepo.GetAsync(new HouseholdId(command.HouseholdId), cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        household.ArchiveChild(new ChildId(command.ChildId));
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Child could not be removed.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after removing child.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result<HouseholdResponse>> UpdateChildAsync(UpdateHouseholdChildCommand command, CancellationToken cancellationToken)
    {
        if (!CanAccess(command.HouseholdId)) return Result<HouseholdResponse>.Fail("Household access denied.", ResultTypeEnum.Forbidden);

        var household = await householdRepo.GetAsync(new HouseholdId(command.HouseholdId), cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        household.UpdateChild(new ChildId(command.ChildId), command.FirstName, command.LastName, command.DateOfBirth);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Child could not be updated.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after updating child.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    private bool CanAccess(Guid householdId)
    {
        return householdId != Guid.Empty && user.CanAccess(new HouseholdId(householdId));
    }
}
