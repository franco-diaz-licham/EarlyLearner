using EarlyLearner.Application.Ports;
using EarlyLearner.Application.UseCases.CoreContext;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using Moq;
using Shouldly;

namespace EarlyLearner.Application.Tests.CoreContext;

[TestFixture]
public sealed class StoredFileQueryServiceTests
{
    private Mock<IStoredFileQueryRepository> _storedFileRepo = default!;
    private Mock<ICurrentUser> _currentUser = default!;
    private Mock<IFileStorageService> _fileStorageService = default!;
    private StoredFileQueryService _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _storedFileRepo = new Mock<IStoredFileQueryRepository>(MockBehavior.Strict);
        _currentUser = new Mock<ICurrentUser>(MockBehavior.Strict);
        _fileStorageService = new Mock<IFileStorageService>(MockBehavior.Strict);

        _sut = new StoredFileQueryService(_storedFileRepo.Object, _currentUser.Object, _fileStorageService.Object);
    }

    [Test]
    public async Task ListAsync_Should_ReturnFiles_ForCurrentHousehold()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var files = new List<StoredFileResponse> {
            CreateResponse(householdId)
        };

        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _storedFileRepo
            .Setup(repo => repo.ListAsync(householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(files);

        // Act
        var result = await _sut.ListAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.ShouldBe(files);
        result.TotalCount.ShouldBe(files.Count);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _storedFileRepo.Verify(repo => repo.ListAsync(householdId, It.IsAny<CancellationToken>()), Times.Once);
        _storedFileRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _fileStorageService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetAsync_Should_ReturnNotFound_On_FileFromAnotherHousehold()
    {
        // Arrange
        var storedFileId = new StoredFileId(Guid.NewGuid());
        var currentHouseholdId = new HouseholdId(Guid.NewGuid());
        var otherHouseholdId = new HouseholdId(Guid.NewGuid());
        var response = CreateResponse(otherHouseholdId) with { StoredFileId = storedFileId.Value };

        _storedFileRepo
            .Setup(repo => repo.GetResponseAsync(storedFileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(currentHouseholdId);

        // Act
        var result = await _sut.GetAsync(storedFileId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.NotFound);
        result.Error!.Message.ShouldBe("Stored file was not found.");
        _storedFileRepo.Verify(repo => repo.GetResponseAsync(storedFileId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _storedFileRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _fileStorageService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DownloadAsync_Should_ReturnDownload_On_FileInCurrentHousehold()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var storedFileId = new StoredFileId(Guid.NewGuid());
        var response = CreateResponse(householdId) with { StoredFileId = storedFileId.Value };
        var stream = new MemoryStream([1, 2, 3]);

        _storedFileRepo
            .Setup(repo => repo.GetResponseAsync(storedFileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _fileStorageService
            .Setup(service => service.DownloadAsync(response.StorageKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stream);

        // Act
        var result = await _sut.DownloadAsync(storedFileId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Success);
        result.Value.FileName.ShouldBe(response.FileName);
        result.Value.ContentType.ShouldBe(response.ContentType);
        result.Value.Content.ShouldBe(stream);
        _storedFileRepo.Verify(repo => repo.GetResponseAsync(storedFileId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _fileStorageService.Verify(service => service.DownloadAsync(response.StorageKey, It.IsAny<CancellationToken>()), Times.Once);
        _storedFileRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _fileStorageService.VerifyNoOtherCalls();
    }

    private static StoredFileResponse CreateResponse(HouseholdId householdId)
    {
        return new StoredFileResponse(Guid.NewGuid(), householdId.Value, "uploads/avatar.png", "avatar.png", "image/png", 1024, StoredFileMediaTypeEnum.Photo, StoredFileStatusEnum.Available, DateTimeOffset.UtcNow);
    }
}

