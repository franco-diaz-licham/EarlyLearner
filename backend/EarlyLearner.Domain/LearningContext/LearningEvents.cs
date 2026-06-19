using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;

namespace EarlyLearner.Domain.LearningContext;

public sealed record LearningActivityLogged(DailyLogId DailyLogId, CompletedActivityId ActivityId, DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record ObservationRecorded(ObservationId ObservationId, ChildId ChildId, DateTimeOffset OccurredAt) : IDomainEvent;
