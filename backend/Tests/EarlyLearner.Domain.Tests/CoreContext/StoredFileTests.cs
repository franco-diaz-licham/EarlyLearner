using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using Shouldly;

namespace EarlyLearner.Domain.Tests.CoreContext;

[TestFixture]
public sealed class StoredFileTests
{
    [Test]
    public void Create_ShouldTrimMetadataAndStartPending()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());
        var uploadedAt = DateTimeOffset.UtcNow;

        // Act
        var storedFile = StoredFile.Create(householdId, " photos/mia.png ", " mia.png ", " image/png ", 128, StoredFileMediaTypeEnum.Photo, uploadedAt);

        // Assert
        storedFile.Id.Value.ShouldNotBe(Guid.Empty);
        storedFile.HouseholdId.ShouldBe(householdId);
        storedFile.StorageKey.ShouldBe("photos/mia.png");
        storedFile.FileName.ShouldBe("mia.png");
        storedFile.ContentType.ShouldBe("image/png");
        storedFile.SizeInBytes.ShouldBe(128);
        storedFile.MediaType.ShouldBe(StoredFileMediaTypeEnum.Photo);
        storedFile.UploadedAt.ShouldBe(uploadedAt);
        storedFile.Status.ShouldBe(StoredFileStatusEnum.Pending);
        storedFile.CreatedOn.ShouldNotBe(default);
    }

    [Test]
    public void MarkAvailable_ShouldSetAvailableStatus()
    {
        // Arrange
        var storedFile = CreateStoredFile();

        // Act
        storedFile.MarkAvailable();

        // Assert
        storedFile.Status.ShouldBe(StoredFileStatusEnum.Available);
        storedFile.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void Reject_ShouldSetRejectedStatus()
    {
        // Arrange
        var storedFile = CreateStoredFile();

        // Act
        storedFile.Reject();

        // Assert
        storedFile.Status.ShouldBe(StoredFileStatusEnum.Rejected);
        storedFile.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void Delete_ShouldSetDeletedStatus()
    {
        // Arrange
        var storedFile = CreateStoredFile();

        // Act
        storedFile.Delete();

        // Assert
        storedFile.Status.ShouldBe(StoredFileStatusEnum.Deleted);
        storedFile.UpdatedOn.ShouldNotBeNull();
    }

    [Test]
    public void Create_ShouldThrow_WhenSizeIsNotGreaterThanZero()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());

        // Act
        var exception = Should.Throw<DomainException>(() => StoredFile.Create(householdId, "photos/mia.png", "mia.png", "image/png", 0, StoredFileMediaTypeEnum.Photo, DateTimeOffset.UtcNow));

        // Assert
        exception.Message.ShouldBe("Stored file size must be greater than zero.");
    }

    [Test]
    public void Create_ShouldThrow_WhenStorageKeyIsMissing()
    {
        // Arrange
        var householdId = new HouseholdId(Guid.NewGuid());

        // Act
        var exception = Should.Throw<DomainException>(() => StoredFile.Create(householdId, " ", "mia.png", "image/png", 128, StoredFileMediaTypeEnum.Photo, DateTimeOffset.UtcNow));

        // Assert
        exception.Message.ShouldBe("storageKey is required.");
    }

    private static StoredFile CreateStoredFile()
    {
        return StoredFile.Create(new HouseholdId(Guid.NewGuid()), "photos/mia.png", "mia.png", "image/png", 128, StoredFileMediaTypeEnum.Photo, DateTimeOffset.UtcNow);
    }
}
