using System.Net;
using System.Net.Http.Json;
using EarlyLearner.Api.Tests.Fixtures;
using EarlyLearner.Domain.LearningContext;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace EarlyLearner.Api.Tests.LearningContext;

[TestFixture]
public sealed class LearningOutcomeEndpointsTests : ApiEndpointTestFixture
{
    [Test]
    public async Task CreateLearningOutcome_Should_CreateOutcomeForCurrentHousehold()
    {
        // Arrange
        await EnsureSessionAsync();
        var request = new {
            Code = "social-turn-taking",
            Name = "Takes turns with others",
            Description = "Practises waiting, sharing and turn taking.",
            Category = "Social",
            SortOrder = 20
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/learning-outcomes", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var db = CreateContext();
        var household = await db.Households.SingleAsync();
        var outcome = await db.LearningOutcomes.SingleAsync();
        outcome.HouseholdId.ShouldBe(household.Id);
        outcome.Name.ShouldBe("Takes turns with others");
        outcome.Status.ShouldBe(LearningOutcomeStatusEnum.Active);
    }

    [Test]
    public async Task UpdateLearningOutcomeStatus_Should_UpdateOutcomeStatus()
    {
        // Arrange
        await EnsureSessionAsync();
        var household = await Db.Households.SingleAsync();
        var outcome = Domain.LearningContext.Entities.LearningOutcome.Create(
            household.Id,
            "motor-mark-making",
            "Uses early mark making",
            "Uses crayons and pencils to make controlled marks.",
            "Motor",
            30);
        Db.LearningOutcomes.Add(outcome);
        await Db.SaveChangesAsync();

        var request = new {
            Status = LearningOutcomeStatusEnum.Archived
        };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/learning-outcomes/{outcome.Id.Value}/status", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

        await using var db = CreateContext();
        var updated = await db.LearningOutcomes.SingleAsync();
        updated.Status.ShouldBe(LearningOutcomeStatusEnum.Archived);
    }
}
