using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.ReadinessContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using FluentValidation;

namespace EarlyLearner.Api.Endpoints;

public static class ReadinessProfileEndpoints
{
    public static IEndpointRouteBuilder MapReadinessProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var profiles = endpoints.MapGroup("/readiness-profiles").WithTags("Readiness profiles");

        profiles.MapGet("/", ListReadinessProfiles).WithName(nameof(ListReadinessProfiles));
        profiles.MapGet("/{readinessProfileId:guid}", GetReadinessProfile).WithName(nameof(GetReadinessProfile));
        profiles.MapPost("/", CreateReadinessProfile).WithName(nameof(CreateReadinessProfile));
        profiles.MapDelete("/{readinessProfileId:guid}", DeleteReadinessProfile).WithName(nameof(DeleteReadinessProfile));

        return endpoints;
    }

    public static async Task<IResult> ListReadinessProfiles(IReadinessProfileQueryService queryService, CancellationToken cancellationToken = default)
    {
        var result = await queryService.ListAsync(cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> GetReadinessProfile(Guid readinessProfileId, IReadinessProfileQueryService queryService, CancellationToken cancellationToken = default)
    {
        if (readinessProfileId == Guid.Empty) return Result<ReadinessProfileResponse>.Fail("Readiness profile id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await queryService.GetAsync(new ReadinessProfileId(readinessProfileId), cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> CreateReadinessProfile(CreateReadinessProfileRequest request, IValidator<CreateReadinessProfileRequest> validator, IReadinessProfileCommandService commandService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new CreateReadinessProfileCommand(
            ChildId: new ChildId(request.ChildId),
            ReadinessOutcomeIds: request.ReadinessOutcomeIds.Select(id => new ReadinessOutcomeId(id)).ToList());

        var result = await commandService.CreateAsync(command, cancellationToken);
        var locationUrl = result.IsSuccess ? $"/readiness-profiles/{result.Value.ReadinessProfileId}" : null;
        return result.ToApiResult(locationUrl);
    }

    public static async Task<IResult> DeleteReadinessProfile(Guid readinessProfileId, IReadinessProfileCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (readinessProfileId == Guid.Empty) return Result.Fail("Readiness profile id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await commandService.DeleteAsync(new ReadinessProfileId(readinessProfileId), cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record CreateReadinessProfileRequest(Guid ChildId, IReadOnlyList<Guid> ReadinessOutcomeIds);

public sealed class CreateReadinessProfileRequestValidator : AbstractValidator<CreateReadinessProfileRequest>
{
    public CreateReadinessProfileRequestValidator()
    {
        RuleFor(request => request.ChildId).NotEmpty();
        RuleFor(request => request.ReadinessOutcomeIds).NotNull();
    }
}
