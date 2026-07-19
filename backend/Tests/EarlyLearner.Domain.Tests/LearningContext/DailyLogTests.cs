using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;
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
    public void Create_ShouldSetOwnershipAndRaiseTraceEvent()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var childId = new ChildId(Guid.NewGuid());
        var logDate = new DateOnly(2026, 7, 18);

        // Act
        var dailyLog = DailyLog.Create(householdId, childId, logDate);

        // Assert
        dailyLog.Id.Value.ShouldNotBe(Guid.Empty);
        dailyLog.HouseholdId.ShouldBe(householdId);
        dailyLog.ChildId.ShouldBe(childId);
        dailyLog.LogDate.ShouldBe(logDate);
        dailyLog.CreatedOn.ShouldNotBe(default);
        dailyLog.DomainEvents.OfType<EntityTraceRecordedEvent>().Single().Action.ShouldBe("DailyLogCreated");
    }

    [Test]
    public void RecordLearningMoment_ShouldTrimDetailsDeduplicateOutcomesAndRaiseEvents()
    {
        // Arrange
        var dailyLog = CreateDailyLog();
        var outcome = CreateLearningOutcome("language-listening", "Listens and responds");
        dailyLog.ClearDomainEvents();

        // Act
        var moment = dailyLog.RecordLearningMoment(LearningMomentKindEnum.Activity, " Paint mixing ", " Mixed colours. ", [outcome, outcome]);

        // Assert
        moment.Title.ShouldBe("Paint mixing");
        moment.Notes.ShouldBe("Mixed colours.");
        moment.LearningOutcomes.Single().ShouldBe(outcome);
        dailyLog.LearningMoments.Single().ShouldBe(moment);
        dailyLog.UpdatedOn.ShouldNotBeNull();
        dailyLog.DomainEvents.OfType<LearningMomentRecordedEvent>().Single().LearningMomentId.ShouldBe(moment.Id);
        dailyLog.DomainEvents.OfType<EntityTraceRecordedEvent>().Single().Action.ShouldBe("LearningMomentRecorded");
    }

    [Test]
    public void RecordLearningMoment_ShouldThrow_WhenNoLearningOutcomesAreProvided()
    {
        // Arrange
        var dailyLog = CreateDailyLog();

        // Act
        var exception = Should.Throw<DomainException>(() => dailyLog.RecordLearningMoment(LearningMomentKindEnum.Activity, "Paint mixing", "Mixed colours.", []));

        // Assert
        exception.Message.ShouldBe("Learning moment must target at least one learning outcome.");
    }

    [Test]
    public void UpdateLearningMoment_ShouldUpdateMomentDetailsAndLearningOutcomes_WhenMomentBelongsToDailyLog()
    {
        // Arrange
        var dailyLog = CreateDailyLog();
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
        dailyLog.DomainEvents.OfType<EntityTraceRecordedEvent>().Single().Action.ShouldBe("LearningMomentUpdated");
    }

    [Test]
    public void UpdateLearningMoment_ShouldThrow_WhenMomentDoesNotBelongToDailyLog()
    {
        // Arrange
        var dailyLog = CreateDailyLog();
        var outcome = CreateLearningOutcome("language-listening", "Listens and responds");

        // Act
        var exception = Should.Throw<DomainException>(() => dailyLog.UpdateLearningMoment(new LearningMomentId(Guid.NewGuid()), LearningMomentKindEnum.Reading, "Story", "Notes", [outcome]));

        // Assert
        exception.Message.ShouldBe("Learning moment was not found in this daily log.");
    }

    [Test]
    public void UpdateLearningMoment_ShouldThrow_WhenUpdatedLearningOutcomesAreEmpty()
    {
        // Arrange
        var dailyLog = CreateDailyLog();
        var outcome = CreateLearningOutcome("language-listening", "Listens and responds");
        var moment = dailyLog.RecordLearningMoment(LearningMomentKindEnum.Activity, "Paint mixing", "Mixed colours.", [outcome]);

        // Act
        var exception = Should.Throw<DomainException>(() => dailyLog.UpdateLearningMoment(moment.Id, LearningMomentKindEnum.Activity, "Paint mixing", "Mixed colours.", []));

        // Assert
        exception.Message.ShouldBe("Learning moment must target at least one learning outcome.");
    }

    [Test]
    public void RemoveLearningMoment_ShouldRemoveMomentAndRaiseTraceEvent_WhenMomentBelongsToDailyLog()
    {
        // Arrange
        var dailyLog = CreateDailyLog();
        var outcome = CreateLearningOutcome("language-listening", "Listens and responds");
        var moment = dailyLog.RecordLearningMoment(LearningMomentKindEnum.Activity, "Paint mixing", "Mixed colours.", [outcome]);
        dailyLog.ClearDomainEvents();

        // Act
        var removedMoment = dailyLog.RemoveLearningMoment(moment.Id);

        // Assert
        removedMoment.ShouldBe(moment);
        dailyLog.LearningMoments.ShouldBeEmpty();
        dailyLog.UpdatedOn.ShouldNotBeNull();
        dailyLog.DomainEvents.OfType<EntityTraceRecordedEvent>().Single().Action.ShouldBe("LearningMomentDeleted");
    }

    [Test]
    public void RemoveLearningMoment_ShouldThrow_WhenMomentDoesNotBelongToDailyLog()
    {
        // Arrange
        var dailyLog = CreateDailyLog();

        // Act
        var exception = Should.Throw<DomainException>(() => dailyLog.RemoveLearningMoment(new LearningMomentId(Guid.NewGuid())));

        // Assert
        exception.Message.ShouldBe("Learning moment was not found in this daily log.");
    }

    [Test]
    public void AttachStoredFile_ShouldAddFileOnce_WhenSameFileIsAttachedTwice()
    {
        // Arrange
        var dailyLog = CreateDailyLog();
        var outcome = CreateLearningOutcome("language-listening", "Listens and responds");
        var moment = dailyLog.RecordLearningMoment(LearningMomentKindEnum.Activity, "Paint mixing", "Mixed colours.", [outcome]);
        var storedFile = StoredFile.Create(dailyLog.HouseholdId, "photos/mia.png", "mia.png", "image/png", 128, StoredFileMediaTypeEnum.Photo, DateTimeOffset.UtcNow);

        // Act
        moment.AttachStoredFile(storedFile);
        moment.AttachStoredFile(storedFile);

        // Assert
        moment.StoredFiles.Single().ShouldBe(storedFile);
        moment.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void RecordLearningMoment_ShouldThrow_WhenTitleIsMissing()
    {
        // Arrange
        var dailyLog = CreateDailyLog();
        var outcome = CreateLearningOutcome("language-listening", "Listens and responds");

        // Act
        var exception = Should.Throw<DomainException>(() => dailyLog.RecordLearningMoment(LearningMomentKindEnum.Activity, " ", "Mixed colours.", [outcome]));

        // Assert
        exception.Message.ShouldBe("title is required.");
    }

    private static DailyLog CreateDailyLog()
    {
        return DailyLog.Create(new HouseholdId(Guid.NewGuid()), new ChildId(Guid.NewGuid()), new DateOnly(2026, 7, 18));
    }

    private static LearningOutcome CreateLearningOutcome(string code, string name)
    {
        return LearningOutcome.Create(code, name, "Description", "Language", 10);
    }
}
