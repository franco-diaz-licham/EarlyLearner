using EarlyLearner.Application.Common;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningRecordContext.Entities;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.LearningRecordContext;

public sealed record CreateDailyLogCommand(Guid HouseholdId, Guid ChildId, DateOnly LogDate);

public interface IDailyLogCommandService
{
    Task<Result<DailyLogResponse>> CreateAsync(CreateDailyLogCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid dailyLogId, CancellationToken cancellationToken);
}

public interface IDailyLogCommandRepository
{
    Task<bool> ChildExistsAsync(Guid householdId, Guid childId, CancellationToken cancellationToken);
    Task<DailyLog?> GetAsync(Guid dailyLogId, CancellationToken cancellationToken);
    Task<DailyLogResponse?> GetResponseAsync(Guid dailyLogId, CancellationToken cancellationToken);
    void Add(DailyLog dailyLog);
    void Remove(DailyLog dailyLog);
}

public sealed class DailyLogCommandService(IDailyLogCommandRepository dailyLogRepo, IUnitOfWork uow) : IDailyLogCommandService
{
    public async Task<Result<DailyLogResponse>> CreateAsync(CreateDailyLogCommand command, CancellationToken cancellationToken)
    {
        var childExists = await dailyLogRepo.ChildExistsAsync(command.HouseholdId, command.ChildId, cancellationToken);
        if (!childExists) return Result<DailyLogResponse>.Fail("Child was not found in this household.", ResultTypeEnum.NotFound);

        var log = DailyLog.Create(new HouseholdId(command.HouseholdId), new ChildId(command.ChildId), command.LogDate);
        dailyLogRepo.Add(log);

        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<DailyLogResponse>.Fail("Daily log could not be created.", ResultTypeEnum.Invalid);

        var result = await dailyLogRepo.GetResponseAsync(log.Id.Value, cancellationToken);
        if (result is null) return Result<DailyLogResponse>.Fail("Daily log could not be retrieved after creation.", ResultTypeEnum.Invalid);
        return Result<DailyLogResponse>.Success(result, ResultTypeEnum.Created);
    }

    public async Task<Result> DeleteAsync(Guid dailyLogId, CancellationToken cancellationToken)
    {
        var log = await dailyLogRepo.GetAsync(dailyLogId, cancellationToken);
        if (log is null) return Result.Fail("Daily log was not found.", ResultTypeEnum.NotFound);

        dailyLogRepo.Remove(log);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        return saved
            ? Result.Success(ResultTypeEnum.Success)
            : Result.Fail("Daily log could not be deleted.", ResultTypeEnum.Invalid);
    }
}
