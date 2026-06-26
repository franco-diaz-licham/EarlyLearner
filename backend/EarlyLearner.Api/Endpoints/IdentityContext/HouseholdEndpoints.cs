using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.AuditContext;
using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
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
        households.MapGet("/{householdId:guid}/audit-trail", ListAuditTrail).WithName(nameof(ListAuditTrail));
        households.MapPut("/{householdId:guid}", UpdateHousehold).WithName(nameof(UpdateHousehold));
        households.MapPost("/{householdId:guid}/carer-invitations", InviteCarer).WithName(nameof(InviteCarer));
        households.MapDelete("/{householdId:guid}/carers/{carerId:guid}", RemoveCarer).WithName(nameof(RemoveCarer));
        households.MapPost("/{householdId:guid}/children", AddChild).WithName(nameof(AddChild));
        households.MapPut("/{householdId:guid}/children/{childId:guid}", UpdateChild).WithName(nameof(UpdateChild));
        households.MapDelete("/{householdId:guid}/children/{childId:guid}", RemoveChild).WithName(nameof(RemoveChild));

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

        var result = await queryService.GetAsync(new HouseholdId(householdId), cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> ListAuditTrail(Guid householdId, string? search, IAuditTrailQueryService queryService, CancellationToken cancellationToken = default)
    {
        if (householdId == Guid.Empty) return Result<List<AuditTrailEntryResponse>>.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var result = await queryService.ListAsync(new HouseholdId(householdId), search, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> UpdateHousehold(Guid householdId, UpdateHouseholdRequest request, IValidator<UpdateHouseholdRequest> validator, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (householdId == Guid.Empty) return Result<HouseholdResponse>.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new UpdateHouseholdCommand(HouseholdId: householdId, Name: request.Name);
        var result = await commandService.UpdateAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> InviteCarer(Guid householdId, InviteHouseholdCarerRequest request, IValidator<InviteHouseholdCarerRequest> validator, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (householdId == Guid.Empty) return Result<HouseholdResponse>.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new AddHouseholdCarerCommand(HouseholdId: householdId, Email: request.Email, Role: request.Role);
        var result = await commandService.AddCarerAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> RemoveCarer(Guid householdId, Guid carerId, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (householdId == Guid.Empty) return Result<HouseholdResponse>.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();
        if (carerId == Guid.Empty) return Result<HouseholdResponse>.Fail("Carer id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var command = new RemoveHouseholdCarerCommand(HouseholdId: householdId, CarerId: carerId);
        var result = await commandService.RemoveCarerAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> AddChild(Guid householdId, AddHouseholdChildRequest request, IValidator<AddHouseholdChildRequest> validator, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (householdId == Guid.Empty) return Result<HouseholdResponse>.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new AddHouseholdChildCommand(
            HouseholdId: householdId,
            FirstName: request.FirstName,
            LastName: request.LastName,
            DateOfBirth: request.DateOfBirth);

        var result = await commandService.AddChildAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> RemoveChild(Guid householdId, Guid childId, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (householdId == Guid.Empty) return Result<HouseholdResponse>.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();
        if (childId == Guid.Empty) return Result<HouseholdResponse>.Fail("Child id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var command = new RemoveHouseholdChildCommand(HouseholdId: householdId, ChildId: childId);
        var result = await commandService.RemoveChildAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> UpdateChild(Guid householdId, Guid childId, UpdateHouseholdChildRequest request, IValidator<UpdateHouseholdChildRequest> validator, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (householdId == Guid.Empty) return Result<HouseholdResponse>.Fail("Household id is required.", ResultTypeEnum.Invalid).ToApiResult();
        if (childId == Guid.Empty) return Result<HouseholdResponse>.Fail("Child id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new UpdateHouseholdChildCommand(
            HouseholdId: householdId,
            ChildId: childId,
            FirstName: request.FirstName,
            LastName: request.LastName,
            DateOfBirth: request.DateOfBirth);

        var result = await commandService.UpdateChildAsync(command, cancellationToken);
        return result.ToApiResult();
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

public sealed record InviteHouseholdCarerRequest(string Email, HouseholdRoleEnum Role);

public sealed class InviteHouseholdCarerRequestValidator : AbstractValidator<InviteHouseholdCarerRequest>
{
    public InviteHouseholdCarerRequestValidator()
    {
        RuleFor(request => request.Email).NotEmpty().EmailAddress();
        RuleFor(request => request.Role)
            .IsInEnum()
            .Must(role => role is HouseholdRoleEnum.Caregiver or HouseholdRoleEnum.Viewer)
            .WithMessage("Role must be caregiver or viewer.");
    }
}

public sealed record AddHouseholdChildRequest(string FirstName, string LastName, DateOnly DateOfBirth);

public sealed record UpdateHouseholdChildRequest(string FirstName, string LastName, DateOnly DateOfBirth);

public sealed class AddHouseholdChildRequestValidator : AbstractValidator<AddHouseholdChildRequest>
{
    public AddHouseholdChildRequestValidator()
    {
        RuleFor(request => request.FirstName).NotEmpty();
        RuleFor(request => request.LastName).NotEmpty();
        RuleFor(request => request.DateOfBirth).NotEmpty();
    }
}

public sealed class UpdateHouseholdChildRequestValidator : AbstractValidator<UpdateHouseholdChildRequest>
{
    public UpdateHouseholdChildRequestValidator()
    {
        RuleFor(request => request.FirstName).NotEmpty();
        RuleFor(request => request.LastName).NotEmpty();
        RuleFor(request => request.DateOfBirth).NotEmpty();
    }
}
