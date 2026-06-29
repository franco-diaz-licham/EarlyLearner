using EarlyLearner.Application.Common;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.PlanningContext.Entities;
using EarlyLearner.Domain.PlanningContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.PlanningContext;

public sealed record CreateLearningPlanCommand(ChildId ChildId, DateOnly StartDate, DateOnly EndDate, string Focus);

public sealed record UpdateLearningPlanCommand(LearningPlanId LearningPlanId, DateOnly StartDate, DateOnly EndDate, string Focus);

public interface ILearningPlanCommandService
{
    Task<Result<LearningPlanResponse>> CreateAsync(CreateLearningPlanCommand command, CancellationToken cancellationToken);
    Task<Result<LearningPlanResponse>> UpdateAsync(UpdateLearningPlanCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(LearningPlanId learningPlanId, CancellationToken cancellationToken);
}

public interface ILearningPlanCommandRepository
{
    Task<bool> ChildExistsAsync(HouseholdId householdId, ChildId childId, CancellationToken cancellationToken);
    Task<LearningPlan?> GetAsync(LearningPlanId learningPlanId, CancellationToken cancellationToken);
    Task<LearningPlanResponse?> GetResponseAsync(LearningPlanId learningPlanId, CancellationToken cancellationToken);
    void Add(LearningPlan learningPlan);
    void Remove(LearningPlan learningPlan);
}

public sealed class LearningPlanCommandService(ILearningPlanCommandRepository learningPlanRepo, IUnitOfWork uow, ICurrentUser currentUser) : ILearningPlanCommandService
{
    public async Task<Result<LearningPlanResponse>> CreateAsync(CreateLearningPlanCommand command, CancellationToken cancellationToken)
    {
        var childExists = await learningPlanRepo.ChildExistsAsync(currentUser.HouseholdId, command.ChildId, cancellationToken);
        if (!childExists) return Result<LearningPlanResponse>.Fail("Child was not found in this household.", ResultTypeEnum.NotFound);

        var period = DateRange.Create(command.StartDate, command.EndDate);
        var plan = LearningPlan.Create(currentUser.HouseholdId, command.ChildId, period, command.Focus);

        learningPlanRepo.Add(plan);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<LearningPlanResponse>.Fail("Learning plan could not be created.", ResultTypeEnum.Invalid);

        var result = await learningPlanRepo.GetResponseAsync(plan.Id, cancellationToken);
        if (result is null) return Result<LearningPlanResponse>.Fail("Learning plan could not be retrieved after creation.", ResultTypeEnum.Invalid);
        return Result<LearningPlanResponse>.Success(result, ResultTypeEnum.Created);
    }

    public async Task<Result<LearningPlanResponse>> UpdateAsync(UpdateLearningPlanCommand command, CancellationToken cancellationToken)
    {
        var plan = await learningPlanRepo.GetAsync(command.LearningPlanId, cancellationToken);
        if (plan is null) return Result<LearningPlanResponse>.Fail("Learning plan was not found.", ResultTypeEnum.NotFound);

        var period = DateRange.Create(command.StartDate, command.EndDate);
        plan.UpdateDetails(period, command.Focus);

        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        if (!saved) return Result<LearningPlanResponse>.Fail("Learning plan could not be updated.", ResultTypeEnum.Invalid);

        var result = await learningPlanRepo.GetResponseAsync(plan.Id, cancellationToken);
        if (result is null) return Result<LearningPlanResponse>.Fail("Learning plan could not be retrieved after update.", ResultTypeEnum.Invalid);
        return Result<LearningPlanResponse>.Success(result, ResultTypeEnum.Updated);
    }

    public async Task<Result> DeleteAsync(LearningPlanId learningPlanId, CancellationToken cancellationToken)
    {
        var plan = await learningPlanRepo.GetAsync(learningPlanId, cancellationToken);
        if (plan is null) return Result.Fail("Learning plan was not found.", ResultTypeEnum.NotFound);

        learningPlanRepo.Remove(plan);
        var saved = await uow.SaveChangesAsync(cancellationToken) > 0;
        return saved ? Result.Success(ResultTypeEnum.Success) : Result.Fail("Learning plan could not be deleted.", ResultTypeEnum.Invalid);
    }
}
