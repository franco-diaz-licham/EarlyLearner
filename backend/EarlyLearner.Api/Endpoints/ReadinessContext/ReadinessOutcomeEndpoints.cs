using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.ReadinessContext;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;
using FluentValidation;

namespace EarlyLearner.Api.Endpoints;

public static class ReadinessOutcomeEndpoints
{
    public static IEndpointRouteBuilder MapReadinessOutcomeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var outcomes = endpoints.MapGroup("/readiness-outcomes").WithTags("Readiness outcomes");

        outcomes.MapGet("/", ListReadinessOutcomes).WithName(nameof(ListReadinessOutcomes));
        outcomes.MapGet("/{readinessOutcomeId:guid}", GetReadinessOutcome).WithName(nameof(GetReadinessOutcome));
        outcomes.MapPost("/", CreateReadinessOutcome).WithName(nameof(CreateReadinessOutcome));
        outcomes.MapPut("/{readinessOutcomeId:guid}", UpdateReadinessOutcome).WithName(nameof(UpdateReadinessOutcome));
        outcomes.MapDelete("/{readinessOutcomeId:guid}", DeleteReadinessOutcome).WithName(nameof(DeleteReadinessOutcome));

        return endpoints;
    }

    public static async Task<IResult> ListReadinessOutcomes(IReadinessOutcomeQueryService queryService, CancellationToken cancellationToken = default)
    {
        var result = await queryService.ListAsync(cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> GetReadinessOutcome(Guid readinessOutcomeId, IReadinessOutcomeQueryService queryService, CancellationToken cancellationToken = default)
    {
        if (readinessOutcomeId == Guid.Empty) return Result<ReadinessOutcomeResponse>.Fail("Readiness outcome id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await queryService.GetAsync(new ReadinessOutcomeId(readinessOutcomeId), cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> CreateReadinessOutcome(CreateReadinessOutcomeRequest request, IValidator<CreateReadinessOutcomeRequest> validator, IReadinessOutcomeCommandService commandService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new CreateReadinessOutcomeCommand(
            Code: request.Code,
            Name: request.Name,
            Description: request.Description,
            Category: request.Category,
            SortOrder: request.SortOrder);

        var result = await commandService.CreateAsync(command, cancellationToken);
        var locationUrl = result.IsSuccess ? $"/readiness-outcomes/{result.Value.ReadinessOutcomeId}" : null;
        return result.ToApiResult(locationUrl);
    }

    public static async Task<IResult> UpdateReadinessOutcome(Guid readinessOutcomeId, UpdateReadinessOutcomeRequest request, IValidator<UpdateReadinessOutcomeRequest> validator, IReadinessOutcomeCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (readinessOutcomeId == Guid.Empty) return Result<ReadinessOutcomeResponse>.Fail("Readiness outcome id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new UpdateReadinessOutcomeCommand(
            ReadinessOutcomeId: new ReadinessOutcomeId(readinessOutcomeId),
            Name: request.Name,
            Description: request.Description,
            Category: request.Category,
            SortOrder: request.SortOrder);

        var result = await commandService.UpdateAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> DeleteReadinessOutcome(Guid readinessOutcomeId, IReadinessOutcomeCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (readinessOutcomeId == Guid.Empty) return Result.Fail("Readiness outcome id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await commandService.DeleteAsync(new ReadinessOutcomeId(readinessOutcomeId), cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record CreateReadinessOutcomeRequest(string Code, string Name, string Description, string Category, int SortOrder);

public sealed class CreateReadinessOutcomeRequestValidator : AbstractValidator<CreateReadinessOutcomeRequest>
{
    public CreateReadinessOutcomeRequestValidator()
    {
        RuleFor(request => request.Code).NotEmpty();
        RuleFor(request => request.Name).NotEmpty();
        RuleFor(request => request.Description).NotEmpty();
        RuleFor(request => request.Category).NotEmpty();
        RuleFor(request => request.SortOrder).GreaterThanOrEqualTo(0);
    }
}

public sealed record UpdateReadinessOutcomeRequest(string Name, string Description, string Category, int SortOrder);

public sealed class UpdateReadinessOutcomeRequestValidator : AbstractValidator<UpdateReadinessOutcomeRequest>
{
    public UpdateReadinessOutcomeRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty();
        RuleFor(request => request.Description).NotEmpty();
        RuleFor(request => request.Category).NotEmpty();
        RuleFor(request => request.SortOrder).GreaterThanOrEqualTo(0);
    }
}
