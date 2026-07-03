using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;

namespace EarlyLearner.Domain.ReadinessContext;

public sealed record ReadinessEvidenceAdded(
    ReadinessProfileId ReadinessProfileId,
    ChildId ChildId,
    ReadinessOutcomeId ReadinessOutcomeId,
    DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record ReadinessStatusChanged(
    ReadinessProfileId ReadinessProfileId,
    ChildId ChildId,
    ReadinessOutcomeId ReadinessOutcomeId,
    ReadinessStatusEnum Status,
    DateTimeOffset OccurredAt) : IDomainEvent;
