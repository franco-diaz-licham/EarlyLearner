using System.Net;
using System.Net.Http.Json;
using EarlyLearner.Api.Tests.Fixtures;
using EarlyLearner.Domain.LearningContext;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace EarlyLearner.Api.Tests.LearningContext;

[TestFixture]
public sealed class DailyLogEndpointsTests : ApiEndpointTestFixture
{
    [Test]
    public async Task CreateDailyLog_Should_CreateDailyLogAndLearningMoment()
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
        Db.LearningOutcomes.Add(outcome);
        await Db.SaveChangesAsync();

        var request = new {
            ChildId = child.Id.Value,
            LogDate = new DateOnly(2026, 7, 24),
            Kind = LearningMomentKindEnum.Activity,
            Title = "Paint mixing",
            Notes = "Mixed colours and described the changes.",
            LearningOutcomeIds = new[] { outcome.Id.Value }
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/daily-logs", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var db = CreateContext();
        var dailyLog = await db.DailyLogs
            .Include(log => log.LearningMoments)
            .SingleAsync();
        dailyLog.ChildId.ShouldBe(child.Id);
        dailyLog.LogDate.ShouldBe(new DateOnly(2026, 7, 24));
        dailyLog.LearningMoments.Single().Title.ShouldBe("Paint mixing");
    }
}
