using EarlyLearner.Api.Configuration;
using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Api.Endpoints;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var identity = endpoints.MapGroup("/identity").WithTags("Identity");
        identity.MapPost("/session", EnsureSession).WithName(nameof(EnsureSession));
        return endpoints;
    }

    public static async Task<IResult> EnsureSession(HttpContext context, ICurrentUserProvisioningService provisioningService, CancellationToken cancellationToken = default)
    {
        if (context.User.Identity?.IsAuthenticated != true) return Result<UserModel>.Fail("User is not authenticated.", ResultTypeEnum.Unauthorized).ToApiResult();
        var identity = ExternalUserIdentityReader.Read(context.User);
        if (identity is null) return Result<UserModel>.Fail("Authenticated user is missing required identity claims.", ResultTypeEnum.Unauthorized).ToApiResult();
        var result = await provisioningService.EnsureCurrentUserAsync(identity, cancellationToken);
        return result.ToApiResult();
    }
}
