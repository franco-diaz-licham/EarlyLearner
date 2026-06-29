using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.AuditContext;
using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Domain.IdentityContext;
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
        households.MapGet("/current", GetHousehold).WithName(nameof(GetHousehold));
        households.MapGet("/audit-trail", ListAuditTrail).WithName(nameof(ListAuditTrail));
        households.MapPut("/", UpdateHousehold).WithName(nameof(UpdateHousehold));
        households.MapPost("/carer-invitations", InviteCarer).WithName(nameof(InviteCarer));
        households.MapDelete("/carers/{carerId:guid}", RemoveCarer).WithName(nameof(RemoveCarer));
        households.MapPost("/children", AddChild).WithName(nameof(AddChild));
        households.MapPut("/children/{childId:guid}", UpdateChild).WithName(nameof(UpdateChild));
        households.MapDelete("/children/{childId:guid}", RemoveChild).WithName(nameof(RemoveChild));

        return endpoints;
    }

    public static async Task<IResult> ListHouseholds(IHouseholdQueryService queryService, CancellationToken cancellationToken = default)
    {
        var result = await queryService.ListAsync(cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> GetHousehold(IHouseholdQueryService queryService, CancellationToken cancellationToken = default)
    {
        var result = await queryService.GetAsync(cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> ListAuditTrail(string? search, IAuditTrailQueryService queryService, CancellationToken cancellationToken = default)
    {
        var result = await queryService.ListAsync(search, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> UpdateHousehold(UpdateHouseholdRequest request, IValidator<UpdateHouseholdRequest> validator, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new UpdateHouseholdCommand(Name: request.Name);
        var result = await commandService.UpdateAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> InviteCarer(InviteHouseholdCarerRequest request, IValidator<InviteHouseholdCarerRequest> validator, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new AddHouseholdCarerCommand(Email: request.Email, Role: request.Role);
        var result = await commandService.AddCarerAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> RemoveCarer(Guid carerId, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (carerId == Guid.Empty) return Result<HouseholdResponse>.Fail("Carer id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var command = new RemoveHouseholdCarerCommand(CarerId: carerId);
        var result = await commandService.RemoveCarerAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> AddChild(AddHouseholdChildRequest request, IValidator<AddHouseholdChildRequest> validator, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new AddHouseholdChildCommand(
            FirstName: request.FirstName,
            LastName: request.LastName,
            DateOfBirth: request.DateOfBirth);

        var result = await commandService.AddChildAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> RemoveChild(Guid childId, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (childId == Guid.Empty) return Result<HouseholdResponse>.Fail("Child id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var command = new RemoveHouseholdChildCommand(ChildId: childId);
        var result = await commandService.RemoveChildAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> UpdateChild(Guid childId, UpdateHouseholdChildRequest request, IValidator<UpdateHouseholdChildRequest> validator, IHouseholdCommandService commandService, CancellationToken cancellationToken = default)
    {
        if (childId == Guid.Empty) return Result<HouseholdResponse>.Fail("Child id is required.", ResultTypeEnum.Invalid).ToApiResult();

        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new UpdateHouseholdChildCommand(
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
