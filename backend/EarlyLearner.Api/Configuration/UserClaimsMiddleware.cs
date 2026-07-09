using System.Security.Claims;
using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace EarlyLearner.Api.Configuration;

public sealed class UserClaimsMiddleware(RequestDelegate next)
{
    private static readonly TimeSpan ClaimsCacheTtl = TimeSpan.FromMinutes(5);
    private const string IdentitySessionPath = "/api/v1/identity/session";

    public async Task Invoke(HttpContext context, ICurrentUserProvisioningService userProvisioningService, ICachingService cache, ILogger<UserClaimsMiddleware> logger)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null) {
            await next(context);
            return;
        }

        if (IsIdentitySessionEndpoint(context)) {
            await next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true) {
            await next(context);
            return;
        }

        var identity = ExternalUserIdentityReader.Read(context.User);
        if (identity is null) {
            logger.LogWarning("Authenticated request is missing an external object id claim.");
            await RejectAsync(context, StatusCodes.Status401Unauthorized, "Invalid user.");
            return;
        }

        var cacheKey = $"user-claims:{identity.ExternalTenantId}:{identity.ExternalObjectId}";
        if (!cache.TryGetValue(cacheKey, out UserModel? userModel)) {
            var userResult = await userProvisioningService.ResolveCurrentUserAsync(identity, context.RequestAborted);
            if (!userResult.IsSuccess) {
                logger.LogWarning("No local user could be resolved for external object id {ObjectId}.", identity.ExternalObjectId);
                await RejectAsync(context, GetStatusCode(userResult.Type), userResult.Error?.Message ?? "User not found.");
                return;
            }

            userModel = userResult.Value;
            cache.Set(cacheKey, userModel, ClaimsCacheTtl);
        }

        EnrichPrincipal(context, userModel!);
        await next(context);
    }

    private static void EnrichPrincipal(HttpContext context, UserModel userModel)
    {
        var claims = new List<Claim>
        {
            new(nameof(UserId), userModel.UserId.Value.ToString()),
            new(nameof(UserAccountStatusEnum), userModel.Status.ToString())
        };

        claims.AddRange(userModel.AccessibleHouseholdIds.Select(householdId => new Claim(nameof(HouseholdId), householdId.Value.ToString())));
        if (userModel.CarerId is not null) claims.Add(new Claim(nameof(CarerId), userModel.CarerId.Value.Value.ToString()));
        context.User.AddIdentity(new ClaimsIdentity(claims));
    }

    private static int GetStatusCode(ResultTypeEnum resultType)
    {
        return resultType switch {
            ResultTypeEnum.Forbidden => StatusCodes.Status403Forbidden,
            ResultTypeEnum.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status401Unauthorized
        };
    }

    private static Task RejectAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(message);
    }

    private static bool IsIdentitySessionEndpoint(HttpContext context)
    {
        return context.Request.Method == HttpMethods.Post
            && context.Request.Path.Equals(IdentitySessionPath, StringComparison.OrdinalIgnoreCase);
    }
}
