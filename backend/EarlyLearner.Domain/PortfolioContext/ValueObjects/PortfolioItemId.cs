namespace EarlyLearner.Domain.PortfolioContext.ValueObjects;

/// <summary>
/// Identifies a portfolio item kept as learning evidence.
/// </summary>
public readonly record struct PortfolioItemId(Guid Value);
