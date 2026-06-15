using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Domain.CoreContext.Entities;

/// <summary>
/// Represents a reusable file stored outside the domain model. The entity owns
/// the file metadata and lifecycle while other contexts reference it by id.
/// </summary>
public sealed class StoredFile : Entity<StoredFileId>
{
    private StoredFile(
        StoredFileId id,
        HouseholdId householdId,
        string storageKey,
        string fileName,
        string contentType,
        long sizeInBytes,
        StoredFileMediaTypeEnum mediaType,
        DateTimeOffset uploadedAt)
        : base(id)
    {
        if (sizeInBytes <= 0)
        {
            throw new DomainException("Stored file size must be greater than zero.");
        }

        HouseholdId = householdId;
        StorageKey = Required(storageKey, nameof(storageKey));
        FileName = Required(fileName, nameof(fileName));
        ContentType = Required(contentType, nameof(contentType));
        SizeInBytes = sizeInBytes;
        MediaType = mediaType;
        UploadedAt = uploadedAt;
        Status = StoredFileStatusEnum.Pending;
    }

    /// <summary>
    /// Household that owns the file and controls access to it.
    /// </summary>
    public HouseholdId HouseholdId { get; }

    /// <summary>
    /// Provider-specific object key used by infrastructure to retrieve the file.
    /// </summary>
    public string StorageKey { get; }

    /// <summary>
    /// Original or display file name shown to carers.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// MIME content type captured when the file was uploaded.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    /// File size recorded at upload time.
    /// </summary>
    public long SizeInBytes { get; }

    /// <summary>
    /// Broad media category used by parent-facing evidence views.
    /// </summary>
    public StoredFileMediaTypeEnum MediaType { get; }

    /// <summary>
    /// Current lifecycle state for validation, display, and deletion workflows.
    /// </summary>
    public StoredFileStatusEnum Status { get; private set; }

    /// <summary>
    /// UTC time the file metadata was created after upload.
    /// </summary>
    public DateTimeOffset UploadedAt { get; }

    public static StoredFile Create(
        HouseholdId householdId,
        string storageKey,
        string fileName,
        string contentType,
        long sizeInBytes,
        StoredFileMediaTypeEnum mediaType,
        DateTimeOffset uploadedAt)
    {
        return new StoredFile(
            new StoredFileId(Guid.NewGuid()),
            householdId,
            storageKey,
            fileName,
            contentType,
            sizeInBytes,
            mediaType,
            uploadedAt);
    }

    public void MarkAvailable()
    {
        Status = StoredFileStatusEnum.Available;
    }

    public void Reject()
    {
        Status = StoredFileStatusEnum.Rejected;
    }

    public void Delete()
    {
        Status = StoredFileStatusEnum.Deleted;
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{name} is required.");
        }

        return value.Trim();
    }
}
