using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.UseCases.LearningContext;

public sealed record DailyLogResponse(
    Guid DailyLogId,
    Guid HouseholdId,
    Guid ChildId,
    DateOnly LogDate,
    int LearningMomentCount,
    IReadOnlyList<LearningMomentResponse> LearningMoments);

public sealed record LearningMomentResponse(
    Guid LearningMomentId,
    LearningMomentKindEnum Kind,
    string Title,
    string Notes,
    IReadOnlyList<Guid> LearningOutcomeIds);

public sealed record LearningMomentFeedResponse(
    Guid LearningMomentId,
    Guid DailyLogId,
    Guid HouseholdId,
    Guid ChildId,
    DateOnly LogDate,
    LearningMomentKindEnum Kind,
    string Title,
    string Notes,
    IReadOnlyList<Guid> LearningOutcomeIds);

public sealed record ListLearningMomentsQuery(
    int PageNumber,
    int PageSize,
    ChildId? ChildId,
    string? SearchTerm);

public interface IDailyLogQueryService
{
    Task<Result<List<DailyLogResponse>>> ListAsync(CancellationToken cancellationToken);
    Task<Result<List<LearningMomentFeedResponse>>> ListLearningMomentsAsync(ListLearningMomentsQuery query, CancellationToken cancellationToken);
    Task<Result<DailyLogResponse>> GetAsync(DailyLogId dailyLogId, CancellationToken cancellationToken);
}

public interface IDailyLogQueryRepository
{
    Task<List<DailyLogResponse>> ListAsync(HouseholdId householdId, CancellationToken cancellationToken);
    Task<(List<LearningMomentFeedResponse> Items, int TotalCount)> ListLearningMomentsAsync(HouseholdId householdId, ListLearningMomentsQuery query, CancellationToken cancellationToken);
    Task<DailyLogResponse?> GetResponseAsync(DailyLogId dailyLogId, CancellationToken cancellationToken);
}

public sealed class DailyLogQueryService(IDailyLogQueryRepository dailyLogRepo, ICurrentUser currentUser) : IDailyLogQueryService
{
    public async Task<Result<List<DailyLogResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var logs = await dailyLogRepo.ListAsync(currentUser.HouseholdId, cancellationToken);
        return Result<List<DailyLogResponse>>.Success(logs, ResultTypeEnum.Success, logs.Count);
    }

    public async Task<Result<List<LearningMomentFeedResponse>>> ListLearningMomentsAsync(ListLearningMomentsQuery query, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await dailyLogRepo.ListLearningMomentsAsync(currentUser.HouseholdId, query, cancellationToken);
        return Result<List<LearningMomentFeedResponse>>.Success(items, ResultTypeEnum.Success, totalCount);
    }

    public async Task<Result<DailyLogResponse>> GetAsync(DailyLogId dailyLogId, CancellationToken cancellationToken)
    {
        var log = await dailyLogRepo.GetResponseAsync(dailyLogId, cancellationToken);
        return log is null
            ? Result<DailyLogResponse>.Fail("Daily log was not found.", ResultTypeEnum.NotFound)
            : Result<DailyLogResponse>.Success(log, ResultTypeEnum.Success);
    }
}
