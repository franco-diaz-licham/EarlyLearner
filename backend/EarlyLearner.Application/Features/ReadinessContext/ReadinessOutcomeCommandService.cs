using EarlyLearner.Application.Common;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.ReadinessContext;

public sealed record CreateReadinessOutcomeCommand(string Code, string Name, string Description, string Category, int SortOrder);

public sealed record UpdateReadinessOutcomeCommand(ReadinessOutcomeId ReadinessOutcomeId, string Name, string Description, string Category, int SortOrder);

public interface IReadinessOutcomeCommandService
{
    Task<Result<ReadinessOutcomeResponse>> CreateAsync(CreateReadinessOutcomeCommand command, CancellationToken cancellationToken);
    Task<Result<ReadinessOutcomeResponse>> UpdateAsync(UpdateReadinessOutcomeCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(ReadinessOutcomeId readinessOutcomeId, CancellationToken cancellationToken);
}

public interface IReadinessOutcomeCommandRepository
{
    Task<ReadinessOutcome?> GetAsync(ReadinessOutcomeId readinessOutcomeId, CancellationToken cancellationToken);
    Task<ReadinessOutcomeResponse?> GetResponseAsync(ReadinessOutcomeId readinessOutcomeId, CancellationToken cancellationToken);
    void Add(ReadinessOutcome readinessOutcome);
}

public sealed class ReadinessOutcomeCommandService(IReadinessOutcomeCommandRepository readinessOutcomeRepo, IUnitOfWork uow) : IReadinessOutcomeCommandService
{
    public async Task<Result<ReadinessOutcomeResponse>> CreateAsync(CreateReadinessOutcomeCommand command, CancellationToken cancellationToken)
    {
        var outcome = ReadinessOutcome.Create(command.Code, command.Name, command.Description, command.Category, command.SortOrder);
        readinessOutcomeRepo.Add(outcome);

        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<ReadinessOutcomeResponse>.Fail("Readiness outcome could not be created.", ResultTypeEnum.Invalid);

        var result = await readinessOutcomeRepo.GetResponseAsync(outcome.Id, cancellationToken);
        if (result is null) return Result<ReadinessOutcomeResponse>.Fail("Readiness outcome could not be retrieved after creation.", ResultTypeEnum.Invalid);
        return Result<ReadinessOutcomeResponse>.Success(result, ResultTypeEnum.Created);
    }

    public async Task<Result<ReadinessOutcomeResponse>> UpdateAsync(UpdateReadinessOutcomeCommand command, CancellationToken cancellationToken)
    {
        var outcome = await readinessOutcomeRepo.GetAsync(command.ReadinessOutcomeId, cancellationToken);
        if (outcome is null) return Result<ReadinessOutcomeResponse>.Fail("Readiness outcome was not found.", ResultTypeEnum.NotFound);

        outcome.UpdateDetails(command.Name, command.Description, command.Category, command.SortOrder);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<ReadinessOutcomeResponse>.Fail("Readiness outcome could not be updated.", ResultTypeEnum.Invalid);

        var result = await readinessOutcomeRepo.GetResponseAsync(outcome.Id, cancellationToken);
        if (result is null) return Result<ReadinessOutcomeResponse>.Fail("Readiness outcome could not be retrieved after update.", ResultTypeEnum.Invalid);
        return Result<ReadinessOutcomeResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result> DeleteAsync(ReadinessOutcomeId readinessOutcomeId, CancellationToken cancellationToken)
    {
        var outcome = await readinessOutcomeRepo.GetAsync(readinessOutcomeId, cancellationToken);
        if (outcome is null) return Result.Fail("Readiness outcome was not found.", ResultTypeEnum.NotFound);

        outcome.Archive();
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        return saved ? Result.Success(ResultTypeEnum.Success) : Result.Fail("Readiness outcome could not be archived.", ResultTypeEnum.Invalid);
    }
}
