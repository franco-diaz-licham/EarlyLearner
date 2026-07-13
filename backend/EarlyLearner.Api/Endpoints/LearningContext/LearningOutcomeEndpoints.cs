using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.UseCases.LearningContext;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using FluentValidation;

namespace EarlyLearner.Api.Endpoints;

public static class LearningOutcomeEndpoints
{
    public static IEndpointRouteBuilder MapLearningOutcomeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var outcomes = endpoints.MapGroup("/learning-outcomes").WithTags("Learning outcomes");

        outcomes.MapGet("/", ListLearningOutcomes).WithName(nameof(ListLearningOutcomes));
        outcomes.MapGet("/{learningOutcomeId:guid}", GetLearningOutcome).WithName(nameof(GetLearningOutcome));
        outcomes.MapPost("/", CreateLearningOutcome).WithName(nameof(CreateLearningOutcome));
        outcomes.MapPut("/{learningOutcomeId:guid}", UpdateLearningOutcome).WithName(nameof(UpdateLearningOutcome));
        outcomes.MapPatch("/{learningOutcomeId:guid}/status", UpdateLearningOutcomeStatus).WithName(nameof(UpdateLearningOutcomeStatus));
        outcomes.MapDelete("/{learningOutcomeId:guid}", DeleteLearningOutcome).WithName(nameof(DeleteLearningOutcome));

        return endpoints;
    }

    public static async Task<IResult> ListLearningOutcomes(ILearningOutcomeQueryService queryService, CancellationToken cancellationToken = default)
    {
        var result = await queryService.ListAsync(cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> GetLearningOutcome(Guid learningOutcomeId, ILearningOutcomeQueryService queryService, CancellationToken cancellationToken = default)
    {
        if (learningOutcomeId == Guid.Empty) return Result<LearningOutcomeResponse>.Fail("Learning outcome id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await queryService.GetAsync(new LearningOutcomeId(learningOutcomeId), cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> CreateLearningOutcome(
        CreateLearningOutcomeRequest request,
        IValidator<CreateLearningOutcomeRequest> validator,
        ILearningOutcomeCommandService commandService,
        CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new CreateLearningOutcomeCommand(
            Code: request.Code,
            Name: request.Name,
            Description: request.Description,
            Category: request.Category,
            SortOrder: request.SortOrder);

        var result = await commandService.CreateAsync(command, cancellationToken);
        var locationUrl = result.IsSuccess ? $"/learning-outcomes/{result.Value.LearningOutcomeId}" : null;
        return result.ToApiResult(locationUrl);
    }

    public static async Task<IResult> UpdateLearningOutcome(
        Guid learningOutcomeId,
        UpdateLearningOutcomeRequest request,
        IValidator<UpdateLearningOutcomeRequest> validator,
        ILearningOutcomeCommandService commandService,
        CancellationToken cancellationToken = default)
    {
        if (learningOutcomeId == Guid.Empty) return Result<LearningOutcomeResponse>.Fail("Learning outcome id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new UpdateLearningOutcomeCommand(
            LearningOutcomeId: new LearningOutcomeId(learningOutcomeId),
            Name: request.Name,
            Description: request.Description,
            Category: request.Category,
            SortOrder: request.SortOrder);

        var result = await commandService.UpdateAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> UpdateLearningOutcomeStatus(
        Guid learningOutcomeId,
        UpdateLearningOutcomeStatusRequest request,
        IValidator<UpdateLearningOutcomeStatusRequest> validator,
        ILearningOutcomeCommandService commandService,
        CancellationToken cancellationToken = default)
    {
        if (learningOutcomeId == Guid.Empty) return Result<LearningOutcomeResponse>.Fail("Learning outcome id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new UpdateLearningOutcomeStatusCommand(LearningOutcomeId: new LearningOutcomeId(learningOutcomeId), Status: request.Status);
        var result = await commandService.UpdateStatusAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> DeleteLearningOutcome(Guid learningOutcomeId, ILearningOutcomeCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (learningOutcomeId == Guid.Empty) return Result.Fail("Learning outcome id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await commandService.DeleteAsync(new LearningOutcomeId(learningOutcomeId), cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record CreateLearningOutcomeRequest(string Code, string Name, string Description, string Category, int SortOrder);

public sealed class CreateLearningOutcomeRequestValidator : AbstractValidator<CreateLearningOutcomeRequest>
{
    public CreateLearningOutcomeRequestValidator()
    {
        RuleFor(request => request.Code).NotEmpty();
        RuleFor(request => request.Name).NotEmpty();
        RuleFor(request => request.Description).NotEmpty();
        RuleFor(request => request.Category).NotEmpty();
        RuleFor(request => request.SortOrder).GreaterThanOrEqualTo(0);
    }
}

public sealed record UpdateLearningOutcomeRequest(string Name, string Description, string Category, int SortOrder);

public sealed class UpdateLearningOutcomeRequestValidator : AbstractValidator<UpdateLearningOutcomeRequest>
{
    public UpdateLearningOutcomeRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty();
        RuleFor(request => request.Description).NotEmpty();
        RuleFor(request => request.Category).NotEmpty();
        RuleFor(request => request.SortOrder).GreaterThanOrEqualTo(0);
    }
}

public sealed record UpdateLearningOutcomeStatusRequest(LearningOutcomeStatusEnum Status);

public sealed class UpdateLearningOutcomeStatusRequestValidator : AbstractValidator<UpdateLearningOutcomeStatusRequest>
{
    public UpdateLearningOutcomeStatusRequestValidator()
    {
        RuleFor(request => request.Status).IsInEnum();
    }
}
