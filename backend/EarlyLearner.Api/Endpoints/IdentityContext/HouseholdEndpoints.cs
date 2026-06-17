using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;
using FluentValidation;

namespace EarlyLearner.Api.Endpoints;

public static class HouseholdEndpoints
{
    public static IEndpointRouteBuilder MapHouseholdEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var households = endpoints.MapGroup("/households").WithTags("Households");

        households.MapGet("/", ListHouseholds).WithName(nameof(ListHouseholds));
        households.MapGet("/{householdId:guid}", GetHousehold).WithName(nameof(GetHousehold));
        households.MapPost("/", CreateHousehold).WithName(nameof(CreateHousehold));
        households.MapPut("/{householdId:guid}", UpdateHousehold).WithName(nameof(UpdateHousehold));
        households.MapDelete("/{householdId:guid}", DeleteHousehold).WithName(nameof(DeleteHousehold));

        return endpoints;
    }

    public static async Task<IResult> ListHouseholds(IHouseholdQueryService queryService, CancellationToken cancellationToken = default)
    {
        var result = await queryService.ListAsync(cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> GetHousehold(Guid householdId, IHouseholdQueryService queryService, CancellationToken cancellationToken = default)
    {
        if (householdId == Guid.Empty) return Result<HouseholdResponse>.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await queryService.GetAsync(householdId, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> CreateHousehold(CreateHouseholdRequest request, IValidator<CreateHouseholdRequest> validator, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new CreateHouseholdCommand(
            Name: request.Name,
            OwnerUserId: request.OwnerUserId,
            OwnerFirstName: request.OwnerFirstName,
            OwnerLastName: request.OwnerLastName);

        var result = await commandService.CreateAsync(command, cancellationToken);
        var locationUrl = result.IsSuccess ? $"/households/{result.Value.HouseholdId}" : null;
        return result.ToApiResult(locationUrl);
    }

    public static async Task<IResult> UpdateHousehold(Guid householdId, UpdateHouseholdRequest request, IValidator<UpdateHouseholdRequest> validator, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (householdId == Guid.Empty) return Result<HouseholdResponse>.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new UpdateHouseholdCommand(
            HouseholdId: householdId,
            Name: request.Name);

        var result = await commandService.UpdateAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> DeleteHousehold(Guid householdId, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (householdId == Guid.Empty) return Result.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await commandService.DeleteAsync(householdId, cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record CreateHouseholdRequest(string Name, Guid OwnerUserId, string OwnerFirstName, string OwnerLastName);

public sealed class CreateHouseholdRequestValidator : AbstractValidator<CreateHouseholdRequest>
{
    public CreateHouseholdRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty();
        RuleFor(request => request.OwnerUserId).NotEmpty();
        RuleFor(request => request.OwnerFirstName).NotEmpty();
        RuleFor(request => request.OwnerLastName).NotEmpty();
    }
}

public sealed record UpdateHouseholdRequest(string Name);

public sealed class UpdateHouseholdRequestValidator : AbstractValidator<UpdateHouseholdRequest>
{
    public UpdateHouseholdRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty();
    }
}
