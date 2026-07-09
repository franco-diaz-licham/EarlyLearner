using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.ReadinessContext;

public sealed record ReadinessProfileResponse(Guid ReadinessProfileId, Guid HouseholdId, Guid ChildId, IReadOnlyList<Guid> ReadinessOutcomeIds);

public interface IReadinessProfileQueryService
{
    Task<Result<List<ReadinessProfileResponse>>> ListAsync(CancellationToken cancellationToken);
    Task<Result<ReadinessProfileResponse>> GetAsync(ReadinessProfileId readinessProfileId, CancellationToken cancellationToken);
}

public interface IReadinessProfileQueryRepository
{
    Task<List<ReadinessProfileResponse>> ListAsync(HouseholdId householdId, CancellationToken cancellationToken);
    Task<ReadinessProfileResponse?> GetResponseAsync(ReadinessProfileId readinessProfileId, CancellationToken cancellationToken);
}

public sealed class ReadinessProfileQueryService(IReadinessProfileQueryRepository readinessProfileRepo, ICurrentUser currentUser) : IReadinessProfileQueryService
{
    public async Task<Result<List<ReadinessProfileResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var profiles = await readinessProfileRepo.ListAsync(currentUser.HouseholdId, cancellationToken);
        return Result<List<ReadinessProfileResponse>>.Success(profiles, ResultTypeEnum.Success, profiles.Count);
    }

    public async Task<Result<ReadinessProfileResponse>> GetAsync(ReadinessProfileId readinessProfileId, CancellationToken cancellationToken)
    {
        var profile = await readinessProfileRepo.GetResponseAsync(readinessProfileId, cancellationToken);
        return profile is null
            ? Result<ReadinessProfileResponse>.Fail("Readiness profile was not found.", ResultTypeEnum.NotFound)
            : Result<ReadinessProfileResponse>.Success(profile, ResultTypeEnum.Success);
    }
}
