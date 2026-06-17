using EarlyLearner.Application.Common;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.IdentityContext;

public sealed record CreateHouseholdCommand(string Name, Guid OwnerUserId, string OwnerFirstName, string OwnerLastName);

public sealed record UpdateHouseholdCommand(Guid HouseholdId, string Name);

public interface IHouseholdCommandService
{
    Task<Result<HouseholdResponse>> CreateAsync(CreateHouseholdCommand command, CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> UpdateAsync(UpdateHouseholdCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid householdId, CancellationToken cancellationToken);
}

public interface IHouseholdCommandRepository
{
    Task<Household?> GetAsync(Guid householdId, CancellationToken cancellationToken);
    Task<HouseholdResponse?> GetResponseAsync(Guid householdId, CancellationToken cancellationToken);
    void Add(Household household);
    void Remove(Household household);
}

public sealed class HouseholdCommandService(IHouseholdCommandRepository householdRepo, IUnitOfWork uow) : IHouseholdCommandService
{
    public async Task<Result<HouseholdResponse>> CreateAsync(CreateHouseholdCommand command, CancellationToken cancellationToken)
    {
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

    public async Task<Result> DeleteAsync(Guid householdId, CancellationToken cancellationToken)
    {
        var household = await householdRepo.GetAsync(householdId, cancellationToken);
        if (household is null) return Result.Fail("Household was not found.", ResultTypeEnum.NotFound);

        householdRepo.Remove(household);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        return saved
            ? Result.Success(ResultTypeEnum.Success)
            : Result.Fail("Household could not be deleted.", ResultTypeEnum.Invalid);
    }
}
