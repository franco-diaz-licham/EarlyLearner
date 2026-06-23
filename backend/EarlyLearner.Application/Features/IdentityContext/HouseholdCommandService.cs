using EarlyLearner.Application.Common;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;
using EarlyLearner.Application.Ports;

namespace EarlyLearner.Application.Features.IdentityContext;

public sealed record UpdateHouseholdCommand(Guid HouseholdId, string Name);

public sealed record AddHouseholdCarerCommand(Guid HouseholdId, string Email, string FirstName, string LastName, HouseholdRoleEnum Role);

public sealed record RemoveHouseholdCarerCommand(Guid HouseholdId, Guid CarerId);

public sealed record AddHouseholdChildCommand(Guid HouseholdId, string GivenName, DateOnly DateOfBirth);

public sealed record RemoveHouseholdChildCommand(Guid HouseholdId, Guid ChildId);

public interface IHouseholdCommandService
{
    Task<Result<HouseholdResponse>> CreateAsync(string name, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> UpdateAsync(UpdateHouseholdCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> AddCarerAsync(AddHouseholdCarerCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> RemoveCarerAsync(RemoveHouseholdCarerCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> AddChildAsync(AddHouseholdChildCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> RemoveChildAsync(RemoveHouseholdChildCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid householdId, CancellationToken cancellationToken);
}

public interface IHouseholdCommandRepository
{
    Task<Household?> GetAsync(HouseholdId householdId, CancellationToken cancellationToken);
    Task<HouseholdResponse?> GetResponseAsync(HouseholdId householdId, CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    void Add(Household household);
    void AddUser(User user);
    void Remove(Household household);
}

public sealed class HouseholdCommandService(IHouseholdCommandRepository householdRepo, IUnitOfWork uow, ICurrentUser user) : IHouseholdCommandService
{
    private static readonly TimeSpan InvitationLifetime = TimeSpan.FromDays(1);

    public async Task<Result<HouseholdResponse>> CreateAsync(string name, CancellationToken cancellationToken)
    {
        var household = Household.Create(name, user.UserId);
        householdRepo.Add(household);

        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Household could not be created.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after creation.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Created);
    }

    public async Task<Result<HouseholdResponse>> UpdateAsync(UpdateHouseholdCommand command, CancellationToken cancellationToken)
    {
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
        var household = await householdRepo.GetAsync(new HouseholdId(command.HouseholdId), cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        var invitedUser = await householdRepo.GetUserByEmailAsync(command.Email, cancellationToken);
        if (invitedUser is null) {
            var expiresAt = DateTimeOffset.UtcNow.Add(InvitationLifetime);
            household.InviteCarer(command.Email, command.FirstName, command.LastName, command.Role, user.UserId, expiresAt);
        }
        else {
            household.AddCarer(invitedUser.Id, command.Role);
        }

        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Carer invitation could not be saved.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after invitation.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result<HouseholdResponse>> RemoveCarerAsync(RemoveHouseholdCarerCommand command, CancellationToken cancellationToken)
    {
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
        var household = await householdRepo.GetAsync(new HouseholdId(command.HouseholdId), cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        household.AddChild(command.GivenName, command.DateOfBirth);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Child could not be added.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after adding child.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result<HouseholdResponse>> RemoveChildAsync(RemoveHouseholdChildCommand command, CancellationToken cancellationToken)
    {
        var household = await householdRepo.GetAsync(new HouseholdId(command.HouseholdId), cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        household.ArchiveChild(new ChildId(command.ChildId));
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Child could not be removed.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after removing child.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result> DeleteAsync(Guid householdId, CancellationToken cancellationToken)
    {
        var household = await householdRepo.GetAsync(new HouseholdId(householdId), cancellationToken);
        if (household is null) return Result.Fail("Household was not found.", ResultTypeEnum.NotFound);

        householdRepo.Remove(household);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        return saved ? Result.Success(ResultTypeEnum.Success) : Result.Fail("Household could not be deleted.", ResultTypeEnum.Invalid);
    }
}
