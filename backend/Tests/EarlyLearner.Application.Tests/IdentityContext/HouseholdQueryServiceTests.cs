using EarlyLearner.Application.Ports;
using EarlyLearner.Application.UseCases.IdentityContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using Moq;
using Shouldly;

namespace EarlyLearner.Application.Tests.IdentityContext;

[TestFixture]
public sealed class HouseholdQueryServiceTests
{
    private Mock<IHouseholdQueryRepository> _householdRepo = default!;
    private Mock<ICurrentUser> _currentUser = default!;
    private HouseholdQueryService _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _householdRepo = new Mock<IHouseholdQueryRepository>(MockBehavior.Strict);
        _currentUser = new Mock<ICurrentUser>(MockBehavior.Strict);

        _sut = new HouseholdQueryService(_householdRepo.Object, _currentUser.Object);
    }

    [Test]
    public async Task ListAsync_Should_ReturnAccessibleHouseholds()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var accessibleHouseholdIds = new[] { householdId };
        var households = new List<HouseholdResponse> {
            CreateResponse(householdId)
        };

        _currentUser
            .SetupGet(user => user.AccessibleHouseholdIds)
            .Returns(accessibleHouseholdIds);
        _householdRepo
            .Setup(repo => repo.ListAsync(accessibleHouseholdIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(households);

        // Act
        var result = await _sut.ListAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.ShouldBe(households);
        result.TotalCount.ShouldBe(households.Count);
        _currentUser.VerifyGet(user => user.AccessibleHouseholdIds, Times.Once);
        _householdRepo.Verify(repo => repo.ListAsync(accessibleHouseholdIds, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyNoOtherCalls();
        _householdRepo.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetAsync_Should_ReturnHousehold_ForCurrentHousehold()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var response = CreateResponse(householdId);

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _householdRepo
            .Setup(repo => repo.GetResponseAsync(householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _sut.GetAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.ShouldBe(response);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _householdRepo.Verify(repo => repo.GetResponseAsync(householdId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyNoOtherCalls();
        _householdRepo.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetAsync_Should_ReturnNotFound_On_MissingHousehold()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _householdRepo
            .Setup(repo => repo.GetResponseAsync(householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HouseholdResponse?)null);

        // Act
        var result = await _sut.GetAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.NotFound);
        result.Error!.Message.ShouldBe("Household was not found.");
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _householdRepo.Verify(repo => repo.GetResponseAsync(householdId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyNoOtherCalls();
        _householdRepo.VerifyNoOtherCalls();
    }

    private static HouseholdResponse CreateResponse(HouseholdId householdId)
    {
        return new HouseholdResponse(householdId.Value, "Taylor Household", Carers: [], Children: [], Invitations: []);
    }
}

