using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Images;
using Microsoft.AspNetCore.Http;

namespace Application.Services.General.Images;

public class FoundAlertImageSubmissionService : IFoundAlertImageSubmissionService
{
	private readonly IImageProcessingService _imageProcessingService;
	private readonly IAwsS3Client _awsS3Client;
	private readonly IIdConverterService _idConverterService;

	public FoundAlertImageSubmissionService(
		IImageProcessingService imageProcessingService,
		IAwsS3Client awsS3Client,
		IIdConverterService idConverterService)
	{
		_imageProcessingService =
			imageProcessingService ?? throw new ArgumentNullException(nameof(imageProcessingService));
		_awsS3Client = awsS3Client ?? throw new ArgumentNullException(nameof(awsS3Client));
		_idConverterService = idConverterService ?? throw new ArgumentNullException(nameof(idConverterService));
	}

	// TODO: Add multiple images
	public async Task<string> UploadFoundAlertImageAsync(Guid alertId, IFormFile alertImage)
	{
		return await UploadImage(alertId, alertImage);
	}

	private async Task<string> UploadImage(Guid id, IFormFile image)
	{
		await using MemoryStream compressedImage =
			await _imageProcessingService.CompressImageAsync(image.OpenReadStream());

		string hashedPetId = _idConverterService.ConvertGuidToShortId(id);

		AwsS3ImageResponse uploadedImage = await _awsS3Client.UploadFoundAlertImageAsync(
			imageStream: compressedImage,
			imageFile: image,
			hashedPetId);

		if (!uploadedImage.Success || uploadedImage.PublicUrl is null)
		{
			throw new InternalServerErrorException(
				"Não foi possível fazer upload da imagem, tente novamente mais tarde.");
		}

		return uploadedImage.PublicUrl;
	}
}