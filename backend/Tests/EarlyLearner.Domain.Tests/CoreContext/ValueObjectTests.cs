using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using Shouldly;

namespace EarlyLearner.Domain.Tests.CoreContext;

[TestFixture]
public sealed class ValueObjectTests
{
    [Test]
    public void IdentifierValueObjects_ShouldExposeWrappedGuid()
    {
        // Arrange
        var id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Act
        var storedFileId = new StoredFileId(id);
        var carerId = new CarerId(id);
        var childId = new ChildId(id);
        var householdId = new HouseholdId(id);
        var householdInvitationId = new HouseholdInvitationId(id);
        var userId = new UserId(id);
        var dailyLogId = new DailyLogId(id);
        var learningMomentId = new LearningMomentId(id);
        var learningOutcomeId = new LearningOutcomeId(id);

        // Assert
        storedFileId.Value.ShouldBe(id);
        carerId.Value.ShouldBe(id);
        childId.Value.ShouldBe(id);
        householdId.Value.ShouldBe(id);
        householdInvitationId.Value.ShouldBe(id);
        userId.Value.ShouldBe(id);
        dailyLogId.Value.ShouldBe(id);
        learningMomentId.Value.ShouldBe(id);
        learningOutcomeId.Value.ShouldBe(id);
    }

    [Test]
    public void IdentifierValueObjects_ShouldUseValueEquality_WhenWrappedGuidMatches()
    {
        // Arrange
        var id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        // Act
        var first = new ChildId(id);
        var second = new ChildId(id);

        // Assert
        first.ShouldBe(second);
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }
}
