using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EarlyLearner.Application.Ports;
using System.Net.Mime;
using System.Security.Cryptography;

namespace EarlyLearner.Infrastructure.Ports;

public sealed class AzureFileStorageService(BlobContainerClient containerClient) : IFileStorageService
{
    private readonly BlobContainerClient _container = containerClient;

    public async Task<string> UploadAsync(string key, ContentType contentType, Stream stream, CancellationToken cancellationToken)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (contentType is null) throw new ArgumentNullException(nameof(contentType));
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (!stream.CanRead) throw new ArgumentException("Stream must be readable.", nameof(stream));

        if (stream.CanSeek) stream.Position = 0;
        var blobClient = _container.GetBlobClient(key);

        // Compute SHA256 hash while streaming upload to avoid buffering large files in memory.
        using var sha256 = SHA256.Create();
        using var hashingStream = new CryptoStream(stream, sha256, CryptoStreamMode.Read, leaveOpen: true);
        var headers = new BlobHttpHeaders { ContentType = contentType.MediaType };
        var uploadOptions = new BlobUploadOptions { HttpHeaders = headers };

        // Upload will read the hashingStream to completion and compute hash.
        await blobClient.UploadAsync(hashingStream, uploadOptions, cancellationToken).ConfigureAwait(false);

        // Ensure any remaining reads are consumed so hash is complete
        if (stream.CanSeek && stream.Position < stream.Length) stream.Position = stream.Length;

        var hashBytes = sha256.Hash ?? Array.Empty<byte>();
        // Return hex string of SHA256 (uppercase). Use Convert.ToHexString (available in .NET)
        return Convert.ToHexString(hashBytes);
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
