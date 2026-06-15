namespace EarlyLearner.Domain.IdentityContext.ValueObjects;

/// <summary>
/// Identifies an ASP.NET Core Identity user inside domain ownership rules.
/// </summary>
public readonly record struct UserId(Guid Value);
