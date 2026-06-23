using System.Security.Claims;
using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace EarlyLearner.Api.Configuration;

public sealed class UserClaimsMiddleware(RequestDelegate next)
{
    private static readonly TimeSpan ClaimsCacheTtl = TimeSpan.FromMinutes(5);

    public async Task Invoke(HttpContext context, ICurrentUserProvisioningService userProvisioningService, ICachingService cache, ILogger<UserClaimsMiddleware> logger)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null) {
            await next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true) {
            await next(context);
            return;
        }

        var identity = ExtractIdentity(context.User);
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

    private static string? ExtractTenantId(ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("tid")
            ?? principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");
    }

    private static ExternalUserIdentity? ExtractIdentity(ClaimsPrincipal principal)
    {
        var objectId = ExtractObjectId(principal);
        var email = ExtractEmail(principal);
        if (string.IsNullOrWhiteSpace(objectId) || string.IsNullOrWhiteSpace(email)) return null;

        var givenName = principal.FindFirstValue(ClaimTypes.GivenName) ?? principal.FindFirstValue("given_name");
        var surname = principal.FindFirstValue(ClaimTypes.Surname) ?? principal.FindFirstValue("family_name");
        var displayName = principal.FindFirstValue("name") ?? principal.Identity?.Name ?? email;
        var (firstName, lastName) = ResolveName(givenName, surname, displayName, email);

        return new ExternalUserIdentity(objectId, ExtractTenantId(principal), email, firstName, lastName);
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

    private static (string FirstName, string LastName) ResolveName(string? givenName, string? surname, string displayName, string email)
    {
        if (!string.IsNullOrWhiteSpace(givenName) && !string.IsNullOrWhiteSpace(surname)) return (givenName.Trim(), surname.Trim());
        if (!string.IsNullOrWhiteSpace(givenName)) return (givenName.Trim(), "User");

        var fallbackName = displayName.Contains('@') ? email.Split('@')[0] : displayName;
        var parts = fallbackName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length switch {
            0 => ("EarlyLearner", "User"),
            1 => (parts[0], "User"),
            _ => (parts[0], string.Join(' ', parts.Skip(1)))
        };
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
}
