using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Domain.LearningContext.Entities;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.UseCases.LearningContext;

public sealed record CreateLearningOutcomeCommand(string Code, string Name, string Description, string Category, int SortOrder);

public sealed record UpdateLearningOutcomeCommand(LearningOutcomeId LearningOutcomeId, string Name, string Description, string Category, int SortOrder);

public sealed record UpdateLearningOutcomeStatusCommand(LearningOutcomeId LearningOutcomeId, LearningOutcomeStatusEnum Status);

public interface ILearningOutcomeCommandService
{
    Task<Result<LearningOutcomeResponse>> CreateAsync(CreateLearningOutcomeCommand command, CancellationToken cancellationToken);
    Task<Result<LearningOutcomeResponse>> UpdateAsync(UpdateLearningOutcomeCommand command, CancellationToken cancellationToken);
    Task<Result<LearningOutcomeResponse>> UpdateStatusAsync(UpdateLearningOutcomeStatusCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(LearningOutcomeId learningOutcomeId, CancellationToken cancellationToken);
}

public interface ILearningOutcomeCommandRepository
{
    Task<LearningOutcome?> GetAsync(HouseholdId householdId, LearningOutcomeId learningOutcomeId, CancellationToken cancellationToken);
    Task<LearningOutcomeResponse?> GetResponseAsync(HouseholdId householdId, LearningOutcomeId learningOutcomeId, CancellationToken cancellationToken);
    Task<bool> IsUsedByLearningMomentAsync(HouseholdId householdId, LearningOutcomeId learningOutcomeId, CancellationToken cancellationToken);
    void Add(LearningOutcome learningOutcome);
    void Remove(LearningOutcome learningOutcome);
}

public sealed class LearningOutcomeCommandService(ILearningOutcomeCommandRepository learningOutcomeRepo, IUnitOfWork uow, ICurrentUser currentUser) : ILearningOutcomeCommandService
{
    public async Task<Result<LearningOutcomeResponse>> CreateAsync(CreateLearningOutcomeCommand command, CancellationToken cancellationToken)
    {
        var householdId = currentUser.HouseholdId;
        var outcome = LearningOutcome.Create(householdId, command.Code, command.Name, command.Description, command.Category, command.SortOrder);
        learningOutcomeRepo.Add(outcome);

        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<LearningOutcomeResponse>.Fail("Learning outcome could not be created.", ResultTypeEnum.Invalid);

        var result = await learningOutcomeRepo.GetResponseAsync(householdId, outcome.Id, cancellationToken);
        if (result is null) return Result<LearningOutcomeResponse>.Fail("Learning outcome could not be retrieved after creation.", ResultTypeEnum.Invalid);
        return Result<LearningOutcomeResponse>.Success(result, ResultTypeEnum.Created);
    }

    public async Task<Result<LearningOutcomeResponse>> UpdateAsync(UpdateLearningOutcomeCommand command, CancellationToken cancellationToken)
    {
        var householdId = currentUser.HouseholdId;
        var outcome = await learningOutcomeRepo.GetAsync(householdId, command.LearningOutcomeId, cancellationToken);
        if (outcome is null) return Result<LearningOutcomeResponse>.Fail("Learning outcome was not found.", ResultTypeEnum.NotFound);

        outcome.UpdateDetails(command.Name, command.Description, command.Category, command.SortOrder);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<LearningOutcomeResponse>.Fail("Learning outcome could not be updated.", ResultTypeEnum.Invalid);

        var result = await learningOutcomeRepo.GetResponseAsync(householdId, outcome.Id, cancellationToken);
        if (result is null) return Result<LearningOutcomeResponse>.Fail("Learning outcome could not be retrieved after update.", ResultTypeEnum.Invalid);
        return Result<LearningOutcomeResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result<LearningOutcomeResponse>> UpdateStatusAsync(UpdateLearningOutcomeStatusCommand command, CancellationToken cancellationToken)
    {
        var householdId = currentUser.HouseholdId;
        var outcome = await learningOutcomeRepo.GetAsync(householdId, command.LearningOutcomeId, cancellationToken);
        if (outcome is null) return Result<LearningOutcomeResponse>.Fail("Learning outcome was not found.", ResultTypeEnum.NotFound);

        outcome.UpdateStatus(command.Status);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<LearningOutcomeResponse>.Fail("Learning outcome status could not be updated.", ResultTypeEnum.Invalid);

        var result = await learningOutcomeRepo.GetResponseAsync(householdId, outcome.Id, cancellationToken);
        if (result is null) return Result<LearningOutcomeResponse>.Fail("Learning outcome could not be retrieved after status update.", ResultTypeEnum.Invalid);
        return Result<LearningOutcomeResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result> DeleteAsync(LearningOutcomeId learningOutcomeId, CancellationToken cancellationToken)
    {
        var householdId = currentUser.HouseholdId;
        var outcome = await learningOutcomeRepo.GetAsync(householdId, learningOutcomeId, cancellationToken);
        if (outcome is null) return Result.Fail("Learning outcome was not found.", ResultTypeEnum.NotFound);

        var isUsed = await learningOutcomeRepo.IsUsedByLearningMomentAsync(householdId, learningOutcomeId, cancellationToken);
        if (isUsed) {
            return Result.Fail("Learning outcome is already used by learning moments. Archive it instead of deleting it.", ResultTypeEnum.Conflict);
        }

        learningOutcomeRepo.Remove(outcome);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        return saved ? Result.Success(ResultTypeEnum.Success) : Result.Fail("Learning outcome could not be deleted.", ResultTypeEnum.Invalid);
    }
}
