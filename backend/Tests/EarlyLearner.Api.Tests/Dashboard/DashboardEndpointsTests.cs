using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EarlyLearner.Api.Tests.Fixtures;
using EarlyLearner.Domain.LearningContext;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace EarlyLearner.Api.Tests.Dashboard;

[TestFixture]
public sealed class DashboardEndpointsTests : ApiEndpointTestFixture
{
    [Test]
    public async Task GetHomeDashboard_Should_ReturnCurrentHouseholdSummary()
    {
        // Arrange
        await EnsureSessionAsync();
        var household = await Db.Households.SingleAsync();
        var child = household.AddChild("Mia", "Diaz", new DateOnly(2021, 3, 14), avatarStoredFileId: null);
        var outcome = Domain.LearningContext.Entities.LearningOutcome.Create(
            household.Id,
            "language-listening",
            "Listens and responds",
            "Listens to short instructions.",
            "Language",
            10);
        var dailyLog = Domain.LearningContext.Entities.DailyLog.Create(household.Id, child.Id, DateOnly.FromDateTime(DateTime.UtcNow));
        dailyLog.RecordLearningMoment(LearningMomentKindEnum.Activity, "Paint mixing", "Mixed colours.", [outcome]);
        Db.LearningOutcomes.Add(outcome);
        Db.DailyLogs.Add(dailyLog);
        await Db.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync("/api/v1/dashboard/home");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("data").GetProperty("children").GetArrayLength().ShouldBe(1);
        json.GetProperty("data").GetProperty("learningCoverage").GetProperty("activeOutcomeCount").GetInt32().ShouldBe(1);
    }
}
