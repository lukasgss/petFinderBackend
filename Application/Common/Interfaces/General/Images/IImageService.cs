namespace Application.Common.Interfaces.General.Images;

public interface IImageService
{
    Task<MemoryStream> CompressImageAsync(Stream image);
}