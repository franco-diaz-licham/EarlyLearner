using System.Security.Claims;
using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using Microsoft.AspNetCore.Authorization;

namespace EarlyLearner.Api.Configuration;

public sealed class UserClaimsMiddleware(RequestDelegate next)
{
    private static readonly TimeSpan ClaimsCacheTtl = TimeSpan.FromMinutes(5);

    public async Task Invoke(HttpContext context, IUserQueryService userQueryService, ICachingService cache, ILogger<UserClaimsMiddleware> logger)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null) {
            await next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true) {
            await next(context);
            return;
        }

        var objectId = ExtractObjectId(context.User);
        if (string.IsNullOrWhiteSpace(objectId)) {
            logger.LogWarning("Authenticated request is missing an external object id claim.");
            await RejectAsync(context, StatusCodes.Status401Unauthorized, "Invalid user.");
            return;
        }

        var cacheKey = $"user-claims:{objectId}";
        if (!cache.TryGetValue(cacheKey, out UserModel? userModel)) {
            userModel = await ResolveUserAsync(userQueryService, objectId, context.User, context.RequestAborted);
            if (userModel is null) {
                logger.LogWarning("No local user could be resolved for external object id {ObjectId}.", objectId);
                await RejectAsync(context, StatusCodes.Status401Unauthorized, "User not found.");
                return;
            }

            cache.Set(cacheKey, userModel, ClaimsCacheTtl);
        }

        EnrichPrincipal(context, userModel!);
        await next(context);
    }

    private static async Task<UserModel?> ResolveUserAsync(IUserQueryService userQueryService, string objectId, ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var objectIdResult = await userQueryService.GetUserByObjectIdAsync(objectId, cancellationToken);
        if (objectIdResult.IsSuccess) return objectIdResult.Value;

        var email = ExtractEmail(principal);
        if (string.IsNullOrWhiteSpace(email)) return null;

        var emailResult = await userQueryService.GetUserByEmailAsync(email, cancellationToken);
        return emailResult.IsSuccess ? emailResult.Value : null;
    }

    private static void EnrichPrincipal(HttpContext context, UserModel userModel)
    {
        var claims = new List<Claim>
        {
            new(nameof(UserId), userModel.UserId.Value.ToString()),
            new(nameof(HouseholdId), userModel.HouseholdId.Value.ToString()),
            new(nameof(UserAccountStatusEnum), userModel.Status.ToString())
        };

        if (userModel.CarerId is not null) claims.Add(new Claim(nameof(CarerId), userModel.CarerId.Value.Value.ToString()));
        context.User.AddIdentity(new ClaimsIdentity(claims));
    }

    private static string? ExtractObjectId(ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("oid")
            ?? principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private static string? ExtractEmail(ClaimsPrincipal principal)
    {
        var candidates = new[] {
            principal.FindFirstValue("preferred_username"),
            principal.FindFirstValue("upn"),
            principal.FindFirstValue("email"),
            principal.FindFirstValue(ClaimTypes.Email),
            principal.Identity?.Name
        };

        return candidates.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value) && value.Contains('@'));
    }

    private static Task RejectAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(message);
    }
}
