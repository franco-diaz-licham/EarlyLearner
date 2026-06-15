using EarlyLearner.Domain.Common;

namespace EarlyLearner.Domain.PortfolioContext.ValueObjects;

/// <summary>
/// References media or document content stored outside the domain model. The
/// domain keeps only safe metadata and a storage key.
/// </summary>
public sealed record AttachmentReference
{
    private AttachmentReference(string storageKey, string fileName, MediaTypeEnum mediaType)
    {
        StorageKey = storageKey;
        FileName = fileName;
        MediaType = mediaType;
    }

    public string StorageKey { get; }

    public string FileName { get; }

    public MediaTypeEnum MediaType { get; }

    public static AttachmentReference Create(string storageKey, string fileName, MediaTypeEnum mediaType)
    {
        return new AttachmentReference(
            Required(storageKey, nameof(storageKey)),
            Required(fileName, nameof(fileName)),
            mediaType);
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
