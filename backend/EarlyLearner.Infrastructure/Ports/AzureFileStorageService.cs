using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EarlyLearner.Application.Ports;
using System.Net.Mime;

namespace EarlyLearner.Infrastructure.Ports;

public sealed class AzureFileStorageService(BlobContainerClient containerClient) : IFileStorageService
{
    private readonly BlobContainerClient _container = containerClient;

    public async Task<string> UploadAsync(string key, ContentType contentType, Stream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(contentType);
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead) throw new ArgumentException("Stream must be readable.", nameof(stream));

        if (stream.CanSeek) stream.Position = 0;
        var blobClient = _container.GetBlobClient(key);

        var headers = new BlobHttpHeaders { ContentType = contentType.MediaType };
        var uploadOptions = new BlobUploadOptions { HttpHeaders = headers };

        var response = await blobClient.UploadAsync(stream, uploadOptions, cancellationToken).ConfigureAwait(false);
        return response.Value.ContentHash is { Length: > 0 } contentHash ? Convert.ToHexString(contentHash) : string.Empty;
    }

    public async Task<Stream> DownloadAsync(string key, CancellationToken cancellationToken)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        var blobClient = _container.GetBlobClient(key);
        var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return response.Value.Content;
    }

    public async Task DeleteAsync(string key, CancellationToken cancellationToken)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        var blobClient = _container.GetBlobClient(key);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
