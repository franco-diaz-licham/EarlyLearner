using EarlyLearner.Domain.PortfolioContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.Common;

namespace EarlyLearner.Domain.PortfolioContext;

public sealed record PortfolioItemAdded(PortfolioItemId PortfolioItemId, ChildId ChildId, DateTimeOffset OccurredAt) : IDomainEvent;
