using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Domain.LearningContext.Entities;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using Shouldly;

namespace EarlyLearner.Domain.Tests.LearningContext;

[TestFixture]
public sealed class DailyLogTests
{
    [Test]
    public void UpdateLearningMoment_ShouldUpdateMomentDetailsAndLearningOutcomes_WhenMomentBelongsToDailyLog()
    {
        // Arrange
        var dailyLog = DailyLog.Create(new HouseholdId(Guid.NewGuid()), new ChildId(Guid.NewGuid()), new DateOnly(2026, 7, 18));
        var originalOutcome = CreateLearningOutcome("language-listening", "Listens and responds");
        var updatedOutcome = CreateLearningOutcome("social-turn-taking", "Takes turns with others");
        var moment = dailyLog.RecordLearningMoment(LearningMomentKindEnum.Activity, " Paint mixing ", " Mixed colours. ", [originalOutcome]);
        dailyLog.ClearDomainEvents();

        // Act
        dailyLog.UpdateLearningMoment(moment.Id, LearningMomentKindEnum.Reading, " Updated story ", " Read a picture book. ", [updatedOutcome]);

        // Assert
        moment.Kind.ShouldBe(LearningMomentKindEnum.Reading);
        moment.Title.ShouldBe("Updated story");
        moment.Notes.ShouldBe("Read a picture book.");
        moment.LearningOutcomes.Single().ShouldBe(updatedOutcome);
        moment.UpdatedOn.ShouldNotBeNull();
        dailyLog.UpdatedOn.ShouldNotBeNull();
        dailyLog.DomainEvents.OfType<EntityTraceRecorded>().Single().Action.ShouldBe("LearningMomentUpdated");
    }

    [Test]
    public void UpdateLearningMoment_ShouldThrow_WhenMomentDoesNotBelongToDailyLog()
    {
        // Arrange
        var dailyLog = DailyLog.Create(new HouseholdId(Guid.NewGuid()), new ChildId(Guid.NewGuid()), new DateOnly(2026, 7, 18));
        var outcome = CreateLearningOutcome("language-listening", "Listens and responds");

        // Act
        var exception = Should.Throw<DomainException>(() => dailyLog.UpdateLearningMoment(new LearningMomentId(Guid.NewGuid()), LearningMomentKindEnum.Reading, "Story", "Notes", [outcome]));

        // Assert
        exception.Message.ShouldBe("Learning moment was not found in this daily log.");
    }

    private static LearningOutcome CreateLearningOutcome(string code, string name)
    {
        return LearningOutcome.Create(code, name, "Description", "Language", 10);
    }
}