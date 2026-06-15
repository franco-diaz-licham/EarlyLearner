using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.Common;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;

namespace EarlyLearner.Domain.ReadinessContext;

public sealed record ReadinessEvidenceAdded(
    ReadinessProfileId ReadinessProfileId,
    ChildId ChildId,
    ReadinessDomainCode DomainCode,
    DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record ReadinessStatusChanged(
    ReadinessProfileId ReadinessProfileId,
    ChildId ChildId,
    ReadinessDomainCode DomainCode,
    ReadinessStatusEnum Status,
    DateTimeOffset OccurredAt) : IDomainEvent;
