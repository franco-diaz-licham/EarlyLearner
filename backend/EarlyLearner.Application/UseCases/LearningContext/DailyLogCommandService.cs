using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Domain.LearningContext.Entities;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.UseCases.LearningContext;

public sealed record CreateDailyLogCommand(
    ChildId ChildId,
    DateOnly LogDate,
    LearningMomentKindEnum Kind,
    string Title,
    string Notes,
    IReadOnlyList<LearningOutcomeId> LearningOutcomeIds);

public sealed record UpdateLearningMomentCommand(
    DailyLogId DailyLogId,
    LearningMomentId LearningMomentId,
    LearningMomentKindEnum Kind,
    string Title,
    string Notes,
    IReadOnlyList<LearningOutcomeId> LearningOutcomeIds);

public interface IDailyLogCommandService
{
    Task<Result<DailyLogResponse>> CreateAsync(CreateDailyLogCommand command, CancellationToken cancellationToken);
    Task<Result<DailyLogResponse>> UpdateLearningMomentAsync(UpdateLearningMomentCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteLearningMomentAsync(DailyLogId dailyLogId, LearningMomentId learningMomentId, CancellationToken cancellationToken);
}

public interface IDailyLogCommandRepository
{
    Task<bool> ChildExistsAsync(HouseholdId householdId, ChildId childId, CancellationToken cancellationToken);
    Task<List<LearningOutcome>> GetLearningOutcomesAsync(HouseholdId householdId, IReadOnlyList<LearningOutcomeId> learningOutcomeIds, CancellationToken cancellationToken);
    Task<DailyLog?> GetByChildAndDateAsync(HouseholdId householdId, ChildId childId, DateOnly logDate, CancellationToken cancellationToken);
    Task<DailyLog?> GetAsync(DailyLogId dailyLogId, CancellationToken cancellationToken);
    Task<DailyLogResponse?> GetResponseAsync(DailyLogId dailyLogId, CancellationToken cancellationToken);
    void Add(DailyLog dailyLog);
    void Remove(DailyLog dailyLog);
    void RemoveLearningMoment(LearningMoment learningMoment);
}

public sealed class DailyLogCommandService(IDailyLogCommandRepository dailyLogRepo, IUnitOfWork uow, ICurrentUser currentUser) : IDailyLogCommandService
{
    public async Task<Result<DailyLogResponse>> CreateAsync(CreateDailyLogCommand command, CancellationToken cancellationToken)
    {
        var householdId = currentUser.HouseholdId;
        var childExists = await dailyLogRepo.ChildExistsAsync(householdId, command.ChildId, cancellationToken);
        if (!childExists) return Result<DailyLogResponse>.Fail("Child was not found in this household.", ResultTypeEnum.NotFound);

        var learningOutcomes = await dailyLogRepo.GetLearningOutcomesAsync(householdId, command.LearningOutcomeIds, cancellationToken);
        if (learningOutcomes.Count != command.LearningOutcomeIds.Distinct().Count()) {
            return Result<DailyLogResponse>.Fail("One or more learning outcomes were not found.", ResultTypeEnum.NotFound);
        }

        var log = await dailyLogRepo.GetByChildAndDateAsync(householdId, command.ChildId, command.LogDate, cancellationToken);
        if (log is null) {
            log = DailyLog.Create(householdId, command.ChildId, command.LogDate);
            dailyLogRepo.Add(log);
        }

        log.RecordLearningMoment(command.Kind, command.Title, command.Notes, learningOutcomes);

        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<DailyLogResponse>.Fail("Daily log could not be created.", ResultTypeEnum.Invalid);

        var result = await dailyLogRepo.GetResponseAsync(log.Id, cancellationToken);
        if (result is null) return Result<DailyLogResponse>.Fail("Daily log could not be retrieved after creation.", ResultTypeEnum.Invalid);
        return Result<DailyLogResponse>.Success(result, ResultTypeEnum.Created);
    }

    public async Task<Result<DailyLogResponse>> UpdateLearningMomentAsync(UpdateLearningMomentCommand command, CancellationToken cancellationToken)
    {
        var log = await dailyLogRepo.GetAsync(command.DailyLogId, cancellationToken);
        if (log is null) return Result<DailyLogResponse>.Fail("Daily log was not found.", ResultTypeEnum.NotFound);
        var householdId = currentUser.HouseholdId;
        if (log.HouseholdId != householdId) return Result<DailyLogResponse>.Fail("Daily log was not found.", ResultTypeEnum.NotFound);

        var learningOutcomes = await dailyLogRepo.GetLearningOutcomesAsync(householdId, command.LearningOutcomeIds, cancellationToken);
        if (learningOutcomes.Count != command.LearningOutcomeIds.Distinct().Count()) {
            return Result<DailyLogResponse>.Fail("One or more learning outcomes were not found.", ResultTypeEnum.NotFound);
        }

        log.UpdateLearningMoment(command.LearningMomentId, command.Kind, command.Title, command.Notes, learningOutcomes);

        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<DailyLogResponse>.Fail("Learning moment could not be updated.", ResultTypeEnum.Invalid);

        var result = await dailyLogRepo.GetResponseAsync(log.Id, cancellationToken);
        if (result is null) return Result<DailyLogResponse>.Fail("Daily log could not be retrieved after update.", ResultTypeEnum.Invalid);
        return Result<DailyLogResponse>.Success(result, ResultTypeEnum.Success);
    }

    public async Task<Result> DeleteLearningMomentAsync(DailyLogId dailyLogId, LearningMomentId learningMomentId, CancellationToken cancellationToken)
    {
        var log = await dailyLogRepo.GetAsync(dailyLogId, cancellationToken);
        if (log is null) return Result.Fail("Daily log was not found.", ResultTypeEnum.NotFound);
        var householdId = currentUser.HouseholdId;
        if (log.HouseholdId != householdId) return Result.Fail("Daily log was not found.", ResultTypeEnum.NotFound);

        var moment = log.RemoveLearningMoment(learningMomentId);
        dailyLogRepo.RemoveLearningMoment(moment);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        return saved
            ? Result.Success(ResultTypeEnum.Success)
            : Result.Fail("Learning moment could not be deleted.", ResultTypeEnum.Invalid);
    }
}
