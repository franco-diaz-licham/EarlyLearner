using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Application.Ports;

/// <summary>
/// Represents the resolved authenticated actor making the current request.
/// Application services consume this instead of touching framework identity types.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Alumno360 internal login account id for the resolved actor.
    /// </summary>
    UserId UserId { get; }

    /// <summary>
    /// School for the active actor context.
    /// </summary>
    HouseholdId HouseholdId { get; }

    /// <summary>
    /// Carer profile represented by the active actor context, or <c>null</c> if not in carer context.
    /// </summary>
    CarerId? CarerId { get; }

    /// <summary>
    /// Whether the HTTP request carries a valid, authenticated identity.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Returns the value of an arbitrary claim, or <c>null</c> if absent.
    /// </summary>
    string? GetClaim(string type);

    /// <summary>
    /// Returns all values for an arbitrary claim type.
    /// </summary>
    IReadOnlyList<string> GetClaims(string type);
}
