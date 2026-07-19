using EarlyLearner.Application.UseCases.LearningContext;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using Moq;
using Shouldly;

namespace EarlyLearner.Application.Tests.LearningContext;

[TestFixture]
public sealed class LearningOutcomeQueryServiceTests
{
    private Mock<ILearningOutcomeQueryRepository> _learningOutcomeRepo = default!;
    private LearningOutcomeQueryService _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _learningOutcomeRepo = new Mock<ILearningOutcomeQueryRepository>(MockBehavior.Strict);

        _sut = new LearningOutcomeQueryService(_learningOutcomeRepo.Object);
    }

    [Test]
    public async Task ListAsync_Should_ReturnLearningOutcomes()
    {
        // Arrange
        var outcomes = new List<LearningOutcomeResponse> {
            CreateResponse()
        };

        _learningOutcomeRepo
            .Setup(repo => repo.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcomes);

        // Act
        var result = await _sut.ListAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.ShouldBe(outcomes);
        result.TotalCount.ShouldBe(outcomes.Count);
        _learningOutcomeRepo.Verify(repo => repo.ListAsync(It.IsAny<CancellationToken>()), Times.Once);
        _learningOutcomeRepo.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetAsync_Should_ReturnNotFound_On_MissingLearningOutcome()
    {
        // Arrange
        var learningOutcomeId = new LearningOutcomeId(Guid.NewGuid());

        _learningOutcomeRepo
            .Setup(repo => repo.GetResponseAsync(learningOutcomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LearningOutcomeResponse?)null);

        // Act
        var result = await _sut.GetAsync(learningOutcomeId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.NotFound);
        result.Error!.Message.ShouldBe("Learning outcome was not found.");
        _learningOutcomeRepo.Verify(repo => repo.GetResponseAsync(learningOutcomeId, It.IsAny<CancellationToken>()), Times.Once);
        _learningOutcomeRepo.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetAsync_Should_ReturnLearningOutcome_On_ExistingLearningOutcome()
    {
        // Arrange
        var learningOutcomeId = new LearningOutcomeId(Guid.NewGuid());
        var response = CreateResponse() with { LearningOutcomeId = learningOutcomeId.Value };

        _learningOutcomeRepo
            .Setup(repo => repo.GetResponseAsync(learningOutcomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _sut.GetAsync(learningOutcomeId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.ShouldBe(response);
        _learningOutcomeRepo.Verify(repo => repo.GetResponseAsync(learningOutcomeId, It.IsAny<CancellationToken>()), Times.Once);
        _learningOutcomeRepo.VerifyNoOtherCalls();
    }

    private static LearningOutcomeResponse CreateResponse()
    {
        return new LearningOutcomeResponse(Guid.NewGuid(), "language-listening", "Listens and responds", "Listens to short instructions.", "Language", 10, LearningOutcomeStatusEnum.Active);
    }
}

