using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.UseCases.LearningContext;

public sealed record LearningOutcomeResponse(Guid LearningOutcomeId, string Code, string Name, string Description, string Category, int SortOrder, LearningOutcomeStatusEnum Status);

public interface ILearningOutcomeQueryService
{
    Task<Result<List<LearningOutcomeResponse>>> ListAsync(CancellationToken cancellationToken);
    Task<Result<LearningOutcomeResponse>> GetAsync(LearningOutcomeId learningOutcomeId, CancellationToken cancellationToken);
}

public interface ILearningOutcomeQueryRepository
{
    Task<List<LearningOutcomeResponse>> ListAsync(HouseholdId householdId, CancellationToken cancellationToken);
    Task<LearningOutcomeResponse?> GetResponseAsync(HouseholdId householdId, LearningOutcomeId learningOutcomeId, CancellationToken cancellationToken);
}

public sealed class LearningOutcomeQueryService(ILearningOutcomeQueryRepository learningOutcomeRepo, ICurrentUser currentUser) : ILearningOutcomeQueryService
{
    public async Task<Result<List<LearningOutcomeResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var outcomes = await learningOutcomeRepo.ListAsync(currentUser.HouseholdId, cancellationToken);
        return Result<List<LearningOutcomeResponse>>.Success(outcomes, ResultTypeEnum.Success, outcomes.Count);
    }

    public async Task<Result<LearningOutcomeResponse>> GetAsync(LearningOutcomeId learningOutcomeId, CancellationToken cancellationToken)
    {
        var outcome = await learningOutcomeRepo.GetResponseAsync(currentUser.HouseholdId, learningOutcomeId, cancellationToken);
        return outcome is null
            ? Result<LearningOutcomeResponse>.Fail("Learning outcome was not found.", ResultTypeEnum.NotFound)
            : Result<LearningOutcomeResponse>.Success(outcome, ResultTypeEnum.Success);
    }
}
