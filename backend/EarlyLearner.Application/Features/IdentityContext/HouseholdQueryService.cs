using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.IdentityContext;

public sealed record HouseholdResponse(Guid HouseholdId, string Name);

public interface IHouseholdQueryService
{
    Task<Result<List<HouseholdResponse>>> ListAsync(CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> GetAsync(Guid householdId, CancellationToken cancellationToken);
}

public interface IHouseholdQueryRepository
{
    Task<List<HouseholdResponse>> ListAsync(CancellationToken cancellationToken);
    Task<HouseholdResponse?> GetResponseAsync(Guid householdId, CancellationToken cancellationToken);
}

public sealed class HouseholdQueryService(IHouseholdQueryRepository householdRepo) : IHouseholdQueryService
{
    public async Task<Result<List<HouseholdResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var households = await householdRepo.ListAsync(cancellationToken);
        return Result<List<HouseholdResponse>>.Success(households, ResultTypeEnum.Success, households.Count);
    }

    public async Task<Result<HouseholdResponse>> GetAsync(Guid householdId, CancellationToken cancellationToken)
    {
        var household = await householdRepo.GetResponseAsync(householdId, cancellationToken);
        return household is null
            ? Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound)
            : Result<HouseholdResponse>.Success(household, ResultTypeEnum.Success);
    }
}
