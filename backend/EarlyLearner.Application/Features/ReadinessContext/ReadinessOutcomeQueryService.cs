using EarlyLearner.Domain.ReadinessContext;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.ReadinessContext;

public sealed record ReadinessOutcomeResponse(Guid ReadinessOutcomeId, string Code, string Name, string Description, string Category, int SortOrder, ReadinessOutcomeStatusEnum Status);

public interface IReadinessOutcomeQueryService
{
    Task<Result<List<ReadinessOutcomeResponse>>> ListAsync(CancellationToken cancellationToken);
    Task<Result<ReadinessOutcomeResponse>> GetAsync(ReadinessOutcomeId readinessOutcomeId, CancellationToken cancellationToken);
}

public interface IReadinessOutcomeQueryRepository
{
    Task<List<ReadinessOutcomeResponse>> ListAsync(CancellationToken cancellationToken);
    Task<ReadinessOutcomeResponse?> GetResponseAsync(ReadinessOutcomeId readinessOutcomeId, CancellationToken cancellationToken);
}

public sealed class ReadinessOutcomeQueryService(IReadinessOutcomeQueryRepository readinessOutcomeRepo) : IReadinessOutcomeQueryService
{
    public async Task<Result<List<ReadinessOutcomeResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var outcomes = await readinessOutcomeRepo.ListAsync(cancellationToken);
        return Result<List<ReadinessOutcomeResponse>>.Success(outcomes, ResultTypeEnum.Success, outcomes.Count);
    }

    public async Task<Result<ReadinessOutcomeResponse>> GetAsync(ReadinessOutcomeId readinessOutcomeId, CancellationToken cancellationToken)
    {
        var outcome = await readinessOutcomeRepo.GetResponseAsync(readinessOutcomeId, cancellationToken);
        return outcome is null
            ? Result<ReadinessOutcomeResponse>.Fail("Readiness outcome was not found.", ResultTypeEnum.NotFound)
            : Result<ReadinessOutcomeResponse>.Success(outcome, ResultTypeEnum.Success);
    }
}
