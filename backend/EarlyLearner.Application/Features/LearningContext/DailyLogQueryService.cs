using EarlyLearner.Application.Ports;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.LearningContext;

public sealed record DailyLogResponse(
    Guid DailyLogId,
    Guid HouseholdId,
    Guid ChildId,
    DateOnly LogDate,
    int CompletedActivityCount,
    int ReadingEntryCount,
    int RoutineEntryCount);

public interface IDailyLogQueryService
{
    Task<Result<List<DailyLogResponse>>> ListAsync(CancellationToken cancellationToken);
    Task<Result<DailyLogResponse>> GetAsync(Guid dailyLogId, CancellationToken cancellationToken);
}

public interface IDailyLogQueryRepository
{
    Task<List<DailyLogResponse>> ListAsync(Guid householdId, CancellationToken cancellationToken);
    Task<DailyLogResponse?> GetResponseAsync(Guid dailyLogId, CancellationToken cancellationToken);
}

public sealed class DailyLogQueryService(IDailyLogQueryRepository dailyLogRepo, ICurrentUser currentUser) : IDailyLogQueryService
{
    public async Task<Result<List<DailyLogResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var logs = await dailyLogRepo.ListAsync(currentUser.HouseholdId.Value, cancellationToken);
        return Result<List<DailyLogResponse>>.Success(logs, ResultTypeEnum.Success, logs.Count);
    }

    public async Task<Result<DailyLogResponse>> GetAsync(Guid dailyLogId, CancellationToken cancellationToken)
    {
        var log = await dailyLogRepo.GetResponseAsync(dailyLogId, cancellationToken);
        return log is null
            ? Result<DailyLogResponse>.Fail("Daily log was not found.", ResultTypeEnum.NotFound)
            : Result<DailyLogResponse>.Success(log, ResultTypeEnum.Success);
    }
}
