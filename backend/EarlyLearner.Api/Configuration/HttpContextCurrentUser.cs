using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using System.Security.Claims;

namespace EarlyLearner.Api.Configuration;

public class HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public UserId UserId => TryGetUserId() ?? throw MissingRequiredClaim(nameof(UserId));
    public HouseholdId HouseholdId => TryGetHouseholdId() ?? throw MissingRequiredClaim(nameof(HouseholdId));
    public IReadOnlyCollection<HouseholdId> AccessibleHouseholdIds => GetHouseholdIds();
    public CarerId? CarerId => TryGetCarerId();
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    public string? GetClaim(string type) => User?.FindFirst(type: type)?.Value;

    public IReadOnlyList<string> GetClaims(string type) => User?.FindAll(type: type).Select(claim => claim.Value).ToArray() ?? [];
    public bool CanAccess(HouseholdId householdId) => AccessibleHouseholdIds.Contains(householdId);

    private static InvalidOperationException MissingRequiredClaim(string claimType) => new($"Required current-user claim '{claimType}' is not available.");

    private UserId? TryGetUserId()
    {
        var value = User?.FindFirstValue(nameof(UserId));
        return Guid.TryParse(input: value, result: out var id) ? new UserId(id) : null;
    }

    private HouseholdId? TryGetHouseholdId()
    {
        var value = User?.FindFirstValue(nameof(HouseholdId));
        return Guid.TryParse(input: value, result: out var id) ? new HouseholdId(id) : null;
    }

    private IReadOnlyCollection<HouseholdId> GetHouseholdIds()
    {
        var householdIds = new List<HouseholdId>();
        foreach (var value in GetClaims(nameof(HouseholdId))) {
            if (Guid.TryParse(value, out var id)) householdIds.Add(new HouseholdId(id));
        }

        return householdIds.Distinct().ToArray();
    }

    private CarerId? TryGetCarerId()
    {
        var value = User?.FindFirstValue(nameof(CarerId));
        return Guid.TryParse(input: value, result: out var id) ? new CarerId(id) : null;
    }
}
