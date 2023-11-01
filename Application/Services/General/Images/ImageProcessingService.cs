using Application.Common.Interfaces.General.Images;
using SixLabors.ImageSharp.Formats.Webp;

namespace Application.Services.General.Images;

public class ImageProcessingService : IImageProcessingService
{
    // This method returns a MemoryStream, so it's important to note that
    // the caller is responsible for ensuring the correct disposal of the
    // stream when done with it.
    public async Task<MemoryStream> CompressImageAsync(Stream imageStream)
    {
        MemoryStream outStream = new();

        using (Image image = await Image.LoadAsync(imageStream))
        {
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            double widthRatio = (double)ImageConfigs.MaxWidth / originalWidth;
            double heightRatio = (double)ImageConfigs.MaxHeight / originalHeight;
            double ratio = Math.Min(widthRatio, heightRatio);

            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            image.Mutate(img => img.Resize(new ResizeOptions()
            {
                Size = new Size(newWidth, newHeight),
                Mode = ResizeMode.Max
            }));

            const int imageQuality = 70;
            await image.SaveAsync(outStream, new WebpEncoder()
            {
                Quality = imageQuality
            });
        }

        outStream.Position = 0;
        return outStream;
    }
}