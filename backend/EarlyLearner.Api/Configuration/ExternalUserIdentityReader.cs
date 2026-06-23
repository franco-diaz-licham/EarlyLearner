using System.Security.Claims;
using EarlyLearner.Application.Features.IdentityContext;

namespace EarlyLearner.Api.Configuration;

public static class ExternalUserIdentityReader
{
    public static ExternalUserIdentity? Read(ClaimsPrincipal principal)
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
}
