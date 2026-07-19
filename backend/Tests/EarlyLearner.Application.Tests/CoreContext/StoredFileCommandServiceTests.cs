using EarlyLearner.Application.Ports;
using EarlyLearner.Application.UseCases.CoreContext;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using Moq;
using Shouldly;
using System.Net.Mime;

namespace EarlyLearner.Application.Tests.CoreContext;

[TestFixture]
public sealed class StoredFileCommandServiceTests
{
    private Mock<IStoredFileCommandRepository> _storedFileRepo = default!;
    private Mock<IUnitOfWork> _uow = default!;
    private Mock<ICurrentUser> _currentUser = default!;
    private Mock<IFileStorageService> _fileStorageService = default!;
    private StoredFileCommandService _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _storedFileRepo = new Mock<IStoredFileCommandRepository>(MockBehavior.Strict);
        _uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _currentUser = new Mock<ICurrentUser>(MockBehavior.Strict);
        _fileStorageService = new Mock<IFileStorageService>(MockBehavior.Strict);

        _sut = new StoredFileCommandService(_storedFileRepo.Object, _uow.Object, _currentUser.Object, _fileStorageService.Object);
    }

    [Test]
    public async Task CreateAsync_Should_ReturnCreatedResult_On_ValidCommand()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var uploadedAt = DateTimeOffset.UtcNow;
        await using var content = new MemoryStream([1, 2, 3]);
        var command = new CreateStoredFileCommand(content, "avatar.png", "image/png", 3, StoredFileMediaTypeEnum.Photo, uploadedAt, "uploads/avatar.png");
        StoredFile? addedFile = null;

        _fileStorageService
            .Setup(service => service.UploadAsync(command.StorageKey!, It.Is<ContentType>(type => type.MediaType == command.ContentType), command.Content, It.IsAny<CancellationToken>()))
            .ReturnsAsync(command.StorageKey!);
        _currentUser
            .SetupGet(user => user.HouseholdId)
            .Returns(householdId);
        _storedFileRepo
            .Setup(repo => repo.Add(It.IsAny<StoredFile>()))
            .Callback<StoredFile>(file => addedFile = file);
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _storedFileRepo
            .Setup(repo => repo.GetResponseAsync(It.IsAny<StoredFileId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new StoredFileResponse(addedFile!.Id.Value, householdId.Value, addedFile.StorageKey, addedFile.FileName, addedFile.ContentType, addedFile.SizeInBytes, addedFile.MediaType, addedFile.Status, addedFile.UploadedAt));

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Created);
        result.Value.StorageKey.ShouldBe(command.StorageKey);
        result.Value.HouseholdId.ShouldBe(householdId.Value);
        addedFile.ShouldNotBeNull();
        addedFile.Status.ShouldBe(StoredFileStatusEnum.Pending);
        _fileStorageService.Verify(service => service.UploadAsync(command.StorageKey!, It.Is<ContentType>(type => type.MediaType == command.ContentType), command.Content, It.IsAny<CancellationToken>()), Times.Once);
        _currentUser.VerifyGet(user => user.HouseholdId, Times.Once);
        _storedFileRepo.Verify(repo => repo.Add(It.IsAny<StoredFile>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _storedFileRepo.Verify(repo => repo.GetResponseAsync(addedFile.Id, It.IsAny<CancellationToken>()), Times.Once);
        _storedFileRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _fileStorageService.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateStatusAsync_Should_ReturnUpdatedResult_On_AvailableStatus()
    {
        // Arrange
        var file = CreateStoredFile();
        var response = CreateResponse(file) with { Status = StoredFileStatusEnum.Available };
        var command = new UpdateStoredFileStatusCommand(file.Id, StoredFileStatusEnum.Available);

        _storedFileRepo
            .Setup(repo => repo.GetAsync(command.StoredFileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _storedFileRepo
            .Setup(repo => repo.GetResponseAsync(file.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _sut.UpdateStatusAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Type.ShouldBe(ResultTypeEnum.Updated);
        result.Value.ShouldBe(response);
        file.Status.ShouldBe(StoredFileStatusEnum.Available);
        _storedFileRepo.Verify(repo => repo.GetAsync(command.StoredFileId, It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _storedFileRepo.Verify(repo => repo.GetResponseAsync(file.Id, It.IsAny<CancellationToken>()), Times.Once);
        _storedFileRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _fileStorageService.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DeleteAsync_Should_ReturnInvalid_On_SaveFailure()
    {
        // Arrange
        var file = CreateStoredFile();

        _storedFileRepo
            .Setup(repo => repo.GetAsync(file.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        _uow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _sut.DeleteAsync(file.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Type.ShouldBe(ResultTypeEnum.Invalid);
        result.Error!.Message.ShouldBe("Stored file could not be deleted.");
        file.Status.ShouldBe(StoredFileStatusEnum.Deleted);
        _storedFileRepo.Verify(repo => repo.GetAsync(file.Id, It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _storedFileRepo.VerifyNoOtherCalls();
        _currentUser.VerifyNoOtherCalls();
        _fileStorageService.VerifyNoOtherCalls();
        _uow.VerifyNoOtherCalls();
    }

    private static StoredFile CreateStoredFile()
    {
        return StoredFile.Create(new HouseholdId(Guid.NewGuid()), "uploads/avatar.png", "avatar.png", "image/png", 1024, StoredFileMediaTypeEnum.Photo, DateTimeOffset.UtcNow);
    }

    private static StoredFileResponse CreateResponse(StoredFile file)
    {
        return new StoredFileResponse(file.Id.Value, file.HouseholdId.Value, file.StorageKey, file.FileName, file.ContentType, file.SizeInBytes, file.MediaType, file.Status, file.UploadedAt);
    }
}

