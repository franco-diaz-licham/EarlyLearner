using System.Net;
using System.Net.Http.Json;
using EarlyLearner.Api.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace EarlyLearner.Api.Tests.IdentityContext;

[TestFixture]
public sealed class HouseholdEndpointsTests : ApiEndpointTestFixture
{
    [Test]
    public async Task UpdateHousehold_Should_UpdateCurrentHousehold()
    {
        // Arrange
        await EnsureSessionAsync();
        var request = new {
            Name = "Diaz Learning Home"
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/v1/households", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

        await using var db = CreateContext();
        var household = await db.Households.SingleAsync();
        household.Name.ShouldBe("Diaz Learning Home");
    }

    [Test]
    public async Task AddChild_Should_AddChildToCurrentHousehold()
    {
        // Arrange
        await EnsureSessionAsync();
        var request = new {
            FirstName = "Mia",
            LastName = "Diaz",
            DateOfBirth = new DateOnly(2021, 3, 14),
            AvatarStoredFileId = (Guid?)null
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/households/children", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

        await using var db = CreateContext();
        var child = await db.Children.SingleAsync();
        child.FirstName.ShouldBe("Mia");
        child.LastName.ShouldBe("Diaz");
    }
}
