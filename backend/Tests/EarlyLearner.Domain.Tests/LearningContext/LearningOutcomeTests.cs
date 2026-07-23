using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Domain.LearningContext.Entities;
using Shouldly;

namespace EarlyLearner.Domain.Tests.LearningContext;

[TestFixture]
public sealed class LearningOutcomeTests
{
    [Test]
    public void Create_ShouldNormalizeCodeTrimDetailsAndStartActive()
    {
        // Arrange
        const string code = " Language-Listening ";
        var householdId = new HouseholdId(Guid.NewGuid());

        // Act
        var outcome = LearningOutcome.Create(householdId, code, " Listens and responds ", " Responds to sounds. ", " Language ", 10);

        // Assert
        outcome.Id.Value.ShouldNotBe(Guid.Empty);
        outcome.HouseholdId.ShouldBe(householdId);
        outcome.Code.ShouldBe("language-listening");
        outcome.Name.ShouldBe("Listens and responds");
        outcome.Description.ShouldBe("Responds to sounds.");
        outcome.Category.ShouldBe("Language");
        outcome.SortOrder.ShouldBe(10);
        outcome.Status.ShouldBe(LearningOutcomeStatusEnum.Active);
        outcome.CreatedOn.ShouldNotBe(default);
    }

    [Test]
    public void UpdateDetails_ShouldTrimDetailsAndSetUpdatedOn()
    {
        // Arrange
        var outcome = CreateOutcome();

        // Act
        outcome.UpdateDetails(" Takes turns ", " Waits during play. ", " Social ", 20);

        // Assert
        outcome.Name.ShouldBe("Takes turns");
        outcome.Description.ShouldBe("Waits during play.");
        outcome.Category.ShouldBe("Social");
        outcome.SortOrder.ShouldBe(20);
        outcome.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void UpdateStatus_ShouldSetRequestedStatus()
    {
        // Arrange
        var outcome = CreateOutcome();

        // Act
        outcome.UpdateStatus(LearningOutcomeStatusEnum.Inactive);
        var inactiveUpdatedOn = outcome.UpdatedOn;
        outcome.UpdateStatus(LearningOutcomeStatusEnum.Active);
        outcome.UpdateStatus(LearningOutcomeStatusEnum.Archived);

        // Assert
        inactiveUpdatedOn.ShouldNotBeNull();
        outcome.Status.ShouldBe(LearningOutcomeStatusEnum.Archived);
        outcome.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void Create_ShouldThrow_WhenCodeIsMissing()
    {
        // Arrange
        const string missingCode = " ";

        // Act
        var exception = Should.Throw<DomainException>(() => LearningOutcome.Create(new HouseholdId(Guid.NewGuid()), missingCode, "Listens", "Description", "Language", 10));

        // Assert
        exception.Message.ShouldBe("code is required.");
    }

    [Test]
    public void UpdateDetails_ShouldThrow_WhenDescriptionIsMissing()
    {
        // Arrange
        var outcome = CreateOutcome();

        // Act
        var exception = Should.Throw<DomainException>(() => outcome.UpdateDetails("Listens", " ", "Language", 10));

        // Assert
        exception.Message.ShouldBe("description is required.");
    }

    private static LearningOutcome CreateOutcome()
    {
        return LearningOutcome.Create(new HouseholdId(Guid.NewGuid()), "language-listening", "Listens and responds", "Responds to sounds.", "Language", 10);
    }
}
