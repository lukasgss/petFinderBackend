using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Images;
using Microsoft.AspNetCore.Http;

namespace Application.Services.General.Images;

public class ImageSubmissionService : IImageSubmissionService
{
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IAwsS3Client _awsS3Client;
    private readonly IIdConverterService _idConverterService;

    public ImageSubmissionService(
        IImageProcessingService imageProcessingService,
        IAwsS3Client awsS3Client,
        IIdConverterService idConverterService)
    {
        _imageProcessingService =
            imageProcessingService ?? throw new ArgumentNullException(nameof(imageProcessingService));
        _awsS3Client = awsS3Client ?? throw new ArgumentNullException(nameof(awsS3Client));
        _idConverterService = idConverterService ?? throw new ArgumentNullException(nameof(idConverterService));
    }

    public async Task<string> UploadPetImageAsync(Guid petId, IFormFile petImage)
    {
        await using MemoryStream compressedImage =
            await _imageProcessingService.CompressImageAsync(petImage.OpenReadStream());

        string hashedPetId = _idConverterService.ConvertGuidToShortId(petId);

        AwsS3ImageResponse uploadedImage = await _awsS3Client.UploadPetImageAsync(
            imageStream: compressedImage,
            imageFile: petImage,
            hashedPetId);

        if (!uploadedImage.Success || uploadedImage.PublicUrl is null)
        {
            throw new InternalServerErrorException(
                "Não foi possível fazer upload da imagem, tente novamente mais tarde.");
        }

        return uploadedImage.PublicUrl;
    }

    public async Task<string> UploadUserImageAsync(Guid userId, IFormFile userImage)
    {
        await using MemoryStream compressedImage =
            await _imageProcessingService.CompressImageAsync(userImage.OpenReadStream());

        string hashedUserId = _idConverterService.ConvertGuidToShortId(userId);

        AwsS3ImageResponse uploadedImage = await _awsS3Client.UploadUserImageAsync(
            imageStream: compressedImage,
            imageFile: userImage,
            hashedUserId);

        if (!uploadedImage.Success || uploadedImage.PublicUrl is null)
        {
            throw new InternalServerErrorException(
                "Não foi possível fazer upload da imagem, tente novamente mais tarde.");
        }

        return uploadedImage.PublicUrl;
    }

    public async Task DeletePetImageAsync(Guid petId)
    {
        string hashedPetId = _idConverterService.ConvertGuidToShortId(petId);

        AwsS3ImageResponse response = await _awsS3Client.DeletePetImageAsync(hashedPetId);
        if (!response.Success)
        {
            throw new InternalServerErrorException(
                "Não foi possível excluir a imagem do animal, tente novamente mais tarde.");
        }
    }
}