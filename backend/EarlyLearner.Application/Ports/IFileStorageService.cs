using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace EarlyLearner.Application.Ports;

public interface IFileStorageService
{
    Task DeleteAsync(string key, CancellationToken cancellationToken);
    Task<Stream> DownloadAsync(string key, CancellationToken cancellationToken);
    Task<string> UploadAsync(string key, ContentType contentType, Stream stream, CancellationToken cancellationToken);
}
