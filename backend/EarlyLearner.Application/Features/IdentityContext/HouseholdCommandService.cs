using EarlyLearner.Application.Common;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.IdentityContext;

public sealed record CreateHouseholdCommand(string Name, Guid OwnerUserId, string OwnerFirstName, string OwnerLastName);

public sealed record UpdateHouseholdCommand(Guid HouseholdId, string Name);

public sealed record AddHouseholdCarerCommand(Guid HouseholdId, string Email, string FirstName, string LastName, HouseholdRoleEnum Role);

public sealed record RemoveHouseholdCarerCommand(Guid HouseholdId, Guid CarerId);

public sealed record AddHouseholdChildCommand(Guid HouseholdId, string GivenName, DateOnly DateOfBirth);

public sealed record RemoveHouseholdChildCommand(Guid HouseholdId, Guid ChildId);

public interface IHouseholdCommandService
{
    Task<Result<HouseholdResponse>> CreateAsync(CreateHouseholdCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> UpdateAsync(UpdateHouseholdCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> AddCarerAsync(AddHouseholdCarerCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> RemoveCarerAsync(RemoveHouseholdCarerCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> AddChildAsync(AddHouseholdChildCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> RemoveChildAsync(RemoveHouseholdChildCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid householdId, CancellationToken cancellationToken);
}

public interface IHouseholdCommandRepository
{
    Task<Household?> GetAsync(Guid householdId, CancellationToken cancellationToken);
    Task<HouseholdResponse?> GetResponseAsync(Guid householdId, CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    void Add(Household household);
    void AddUser(User user);
    void Remove(Household household);
}

public sealed class HouseholdCommandService(IHouseholdCommandRepository householdRepo, IUnitOfWork uow) : IHouseholdCommandService
{
    public async Task<Result<HouseholdResponse>> CreateAsync(CreateHouseholdCommand command, CancellationToken cancellationToken)
    {
        var owner = await householdRepo.GetUserByIdAsync(command.OwnerUserId, cancellationToken);
        if (owner is null) {
            owner = User.CreateActiveParent(new UserId(command.OwnerUserId), $"owner-{command.OwnerUserId:N}@earlylearner.local", command.OwnerFirstName, command.OwnerLastName);
            householdRepo.AddUser(owner);
        }

        var household = Household.Create(command.Name, new UserId(command.OwnerUserId), command.OwnerFirstName, command.OwnerLastName);
        householdRepo.Add(household);

        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Household could not be created.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id.Value, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after creation.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Created);
    }

    public async Task<Result<HouseholdResponse>> UpdateAsync(UpdateHouseholdCommand command, CancellationToken cancellationToken)
    {
        var household = await householdRepo.GetAsync(command.HouseholdId, cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        household.Rename(command.Name);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Household could not be updated.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id.Value, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after update.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result<HouseholdResponse>> AddCarerAsync(AddHouseholdCarerCommand command, CancellationToken cancellationToken)
    {
        var household = await householdRepo.GetAsync(command.HouseholdId, cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        var user = await householdRepo.GetUserByEmailAsync(command.Email, cancellationToken);
        if (user is null) {
            user = User.CreatePendingParent(command.Email, command.FirstName, command.LastName);
            householdRepo.AddUser(user);
        }

        household.AddCarer(user.Id, command.Role);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Carer could not be invited.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id.Value, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after invite.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result<HouseholdResponse>> RemoveCarerAsync(RemoveHouseholdCarerCommand command, CancellationToken cancellationToken)
    {
        var household = await householdRepo.GetAsync(command.HouseholdId, cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        household.RemoveCarer(new CarerId(command.CarerId));
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Carer could not be removed.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id.Value, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after removing carer.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result<HouseholdResponse>> AddChildAsync(AddHouseholdChildCommand command, CancellationToken cancellationToken)
    {
        var household = await householdRepo.GetAsync(command.HouseholdId, cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        household.AddChild(command.GivenName, command.DateOfBirth);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Child could not be added.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id.Value, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after adding child.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result<HouseholdResponse>> RemoveChildAsync(RemoveHouseholdChildCommand command, CancellationToken cancellationToken)
    {
        var household = await householdRepo.GetAsync(command.HouseholdId, cancellationToken);
        if (household is null) return Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound);

        household.ArchiveChild(new ChildId(command.ChildId));
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<HouseholdResponse>.Fail("Child could not be removed.", ResultTypeEnum.Invalid);

        var result = await householdRepo.GetResponseAsync(household.Id.Value, cancellationToken);
        if (result is null) return Result<HouseholdResponse>.Fail("Household could not be retrieved after removing child.", ResultTypeEnum.Invalid);
        return Result<HouseholdResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result> DeleteAsync(Guid householdId, CancellationToken cancellationToken)
    {
        var household = await householdRepo.GetAsync(householdId, cancellationToken);
        if (household is null) return Result.Fail("Household was not found.", ResultTypeEnum.NotFound);

        householdRepo.Remove(household);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        return saved ? Result.Success(ResultTypeEnum.Success) : Result.Fail("Household could not be deleted.", ResultTypeEnum.Invalid);
    }
}
