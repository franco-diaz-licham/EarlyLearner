using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EarlyLearner.Infrastructure.Ports;
using Moq;
using Shouldly;
using System.Net.Mime;

namespace EarlyLearner.Infrastructure.Tests.Ports;

[TestFixture]
public sealed class AzureFileStorageServiceTests
{
    private Mock<BlobClient> _blobClient = default!;
    private Mock<BlobContainerClient> _containerClient = default!;
    private AzureFileStorageService _sut = default!;

    [SetUp]
    public void SetUp()
    {
        _blobClient = new Mock<BlobClient>(MockBehavior.Strict);
        _containerClient = new Mock<BlobContainerClient>(MockBehavior.Strict);

        _sut = new AzureFileStorageService(_containerClient.Object);
    }

    [Test]
    public async Task UploadAsync_Should_UploadStreamWithContentTypeAndReturnContentHash()
    {
        // Arrange
        var key = "children/avatar.png";
        var cancellationToken = CancellationToken.None;
        var stream = new MemoryStream([1, 2, 3]);
        stream.Position = stream.Length;
        var contentType = new ContentType("image/png");
        var contentHash = new byte[] { 0x1A, 0x2B };
        var response = CreateUploadResponse(contentHash);

        _blobClient
            .Setup(client => client.UploadAsync(
                It.Is<Stream>(uploadedStream => ReferenceEquals(uploadedStream, stream) && uploadedStream.Position == 0),
                It.Is<BlobUploadOptions>(options => options.HttpHeaders.ContentType == contentType.MediaType),
                cancellationToken))
            .ReturnsAsync(response);
        _containerClient
            .Setup(client => client.GetBlobClient(key))
            .Returns(_blobClient.Object);

        // Act
        var result = await _sut.UploadAsync(key, contentType, stream, cancellationToken);

        // Assert
        result.ShouldBe("1A2B");
        _containerClient.Verify(client => client.GetBlobClient(key), Times.Once);
        _blobClient.VerifyAll();
        _containerClient.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DownloadAsync_Should_ReturnBlobContentStream()
    {
        // Arrange
        var key = "children/avatar.png";
        var cancellationToken = CancellationToken.None;
        var stream = new MemoryStream([1, 2, 3]);
        var response = CreateDownloadResponse(stream);

        _blobClient
            .Setup(client => client.DownloadStreamingAsync(null, cancellationToken))
            .ReturnsAsync(response);
        _containerClient
            .Setup(client => client.GetBlobClient(key))
            .Returns(_blobClient.Object);

        // Act
        var result = await _sut.DownloadAsync(key, cancellationToken);

        // Assert
        result.ShouldBeSameAs(stream);
        _containerClient.Verify(client => client.GetBlobClient(key), Times.Once);
        _blobClient.VerifyAll();
        _containerClient.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DeleteAsync_Should_DeleteBlobIfExists()
    {
        // Arrange
        var key = "children/avatar.png";
        var cancellationToken = CancellationToken.None;
        var response = Response.FromValue(true, Mock.Of<Response>());

        _blobClient
            .Setup(client => client.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken))
            .ReturnsAsync(response);
        _containerClient
            .Setup(client => client.GetBlobClient(key))
            .Returns(_blobClient.Object);

        // Act
        await _sut.DeleteAsync(key, cancellationToken);

        // Assert
        _containerClient.Verify(client => client.GetBlobClient(key), Times.Once);
        _blobClient.VerifyAll();
        _containerClient.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UploadAsync_Should_ThrowArgumentException_On_UnreadableStream()
    {
        // Act
        var result = async () => await _sut.UploadAsync(
            "children/avatar.png",
            new ContentType("image/png"),
            new UnreadableStream(),
            CancellationToken.None);

        // Assert
        await result.ShouldThrowAsync<ArgumentException>();
        _containerClient.VerifyNoOtherCalls();
        _blobClient.VerifyNoOtherCalls();
    }

    private static Response<BlobContentInfo> CreateUploadResponse(byte[] contentHash)
    {
        var contentInfo = BlobsModelFactory.BlobContentInfo(
            new ETag("\"etag\""),
            DateTimeOffset.UtcNow,
            contentHash,
            "version",
            "encryption-key",
            "encryption-scope",
            0);

        return Response.FromValue(contentInfo, Mock.Of<Response>());
    }

    private static Response<BlobDownloadStreamingResult> CreateDownloadResponse(Stream stream)
    {
        var downloadResult = BlobsModelFactory.BlobDownloadStreamingResult(stream, default!);
        return Response.FromValue(downloadResult, Mock.Of<Response>());
    }

    private sealed class UnreadableStream : Stream
    {
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => 0;
        public override long Position { get => 0; set { } }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}