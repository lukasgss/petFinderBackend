using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Tests.Fakes.Files;

public class EmptyFormFile : IFormFile
{
    public string ContentDisposition => "form-data; name=\"file\"; filename=\"\"";

    public string ContentType => "application/octet-stream";
    public string Name { get; } = "";
    public string FileName => "";

    public long Length => 0;
    public IHeaderDictionary Headers => new HeaderDictionary();
    public Stream OpenReadStream() => new MemoryStream(Array.Empty<byte>());

    public void CopyTo(Stream target)
    {
    }

    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }
}