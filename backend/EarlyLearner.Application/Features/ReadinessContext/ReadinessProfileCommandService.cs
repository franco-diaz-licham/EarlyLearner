using EarlyLearner.Application.Common;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.ReadinessContext;

public sealed record CreateReadinessProfileCommand(ChildId ChildId, IReadOnlyList<ReadinessOutcomeId> ReadinessOutcomeIds);

public interface IReadinessProfileCommandService
{
    Task<Result<ReadinessProfileResponse>> CreateAsync(CreateReadinessProfileCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(ReadinessProfileId readinessProfileId, CancellationToken cancellationToken);
}

public interface IReadinessProfileCommandRepository
{
    Task<bool> ChildExistsAsync(HouseholdId householdId, ChildId childId, CancellationToken cancellationToken);
    Task<List<ReadinessOutcome>> GetReadinessOutcomesAsync(IReadOnlyList<ReadinessOutcomeId> readinessOutcomeIds, CancellationToken cancellationToken);
    Task<ReadinessProfile?> GetAsync(ReadinessProfileId readinessProfileId, CancellationToken cancellationToken);
    Task<ReadinessProfileResponse?> GetResponseAsync(ReadinessProfileId readinessProfileId, CancellationToken cancellationToken);
    void Add(ReadinessProfile readinessProfile);
    void Remove(ReadinessProfile readinessProfile);
}

public sealed class ReadinessProfileCommandService(IReadinessProfileCommandRepository readinessProfileRepo, IUnitOfWork uow, ICurrentUser currentUser) : IReadinessProfileCommandService
{
    public async Task<Result<ReadinessProfileResponse>> CreateAsync(CreateReadinessProfileCommand command, CancellationToken cancellationToken)
    {
        var childExists = await readinessProfileRepo.ChildExistsAsync(currentUser.HouseholdId, command.ChildId, cancellationToken);
        if (!childExists) return Result<ReadinessProfileResponse>.Fail("Child was not found in this household.", ResultTypeEnum.NotFound);

        var outcomes = await readinessProfileRepo.GetReadinessOutcomesAsync(command.ReadinessOutcomeIds, cancellationToken);
        if (outcomes.Count != command.ReadinessOutcomeIds.Distinct().Count()) return Result<ReadinessProfileResponse>.Fail("One or more readiness outcomes were not found.", ResultTypeEnum.NotFound);

        var profile = ReadinessProfile.Create(currentUser.HouseholdId, command.ChildId, outcomes);
        readinessProfileRepo.Add(profile);

        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<ReadinessProfileResponse>.Fail("Readiness profile could not be created.", ResultTypeEnum.Invalid);

        var result = await readinessProfileRepo.GetResponseAsync(profile.Id, cancellationToken);
        if (result is null) return Result<ReadinessProfileResponse>.Fail("Readiness profile could not be retrieved after creation.", ResultTypeEnum.Invalid);
        return Result<ReadinessProfileResponse>.Success(result, ResultTypeEnum.Created);
    }

    public async Task<Result> DeleteAsync(ReadinessProfileId readinessProfileId, CancellationToken cancellationToken)
    {
        var profile = await readinessProfileRepo.GetAsync(readinessProfileId, cancellationToken);
        if (profile is null) return Result.Fail("Readiness profile was not found.", ResultTypeEnum.NotFound);

        readinessProfileRepo.Remove(profile);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        return saved ? Result.Success(ResultTypeEnum.Success) : Result.Fail("Readiness profile could not be deleted.", ResultTypeEnum.Invalid);
    }
}
