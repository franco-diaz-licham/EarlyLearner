using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.CoreContext;

namespace EarlyLearner.Domain.LearningContext;

public sealed record LearningMomentRecordedEvent(DailyLogId DailyLogId, LearningMomentId LearningMomentId, ChildId ChildId, DateTimeOffset OccurredAt) : IDomainEvent;
