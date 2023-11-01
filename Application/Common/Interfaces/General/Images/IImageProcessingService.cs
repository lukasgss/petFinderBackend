namespace Application.Common.Interfaces.General.Images;

public interface IImageProcessingService
{
    Task<MemoryStream> CompressImageAsync(Stream image);
}