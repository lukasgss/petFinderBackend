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

	public async Task<IReadOnlyList<string>> UploadImagesAsync(Guid alertId, List<IFormFile> alertImages)
	{
		return await UploadImages(alertId, alertImages);
	}

	public async Task<IReadOnlyList<string>> UpdateImagesAsync(
		Guid petId, List<IFormFile> newlyAddedImages, int previousImageCount)
	{
		List<string> uploadedImages = await UploadImages(petId, newlyAddedImages);

		if (uploadedImages.Count < previousImageCount)
		{
			await DeletePreviousImagesAsync(petId, uploadedImages.Count, previousImageCount);
		}

		return uploadedImages;
	}

	private async Task DeletePreviousImagesAsync(Guid id, int currentImageCount, int previousImageCount)
	{
		for (int index = currentImageCount; index < previousImageCount; index++)
		{
			string hashedId = _idConverterService.ConvertGuidToShortId(id, index);

			AwsS3ImageResponse response = await _awsS3Client.DeleteFoundAlertImageAsync(hashedId);
			if (!response.Success)
			{
				throw new InternalServerErrorException(
					"Não foi possível excluir a imagem, tente novamente mais tarde.");
			}
		}
	}

	private async Task<List<string>> UploadImages(Guid id, List<IFormFile> images)
	{
		List<string> uploadedImages = new(images.Count);

		for (int index = 0; index < images.Count; index++)
		{
			await using MemoryStream compressedImage =
				await _imageProcessingService.CompressImageAsync(images[index].OpenReadStream());

			string hashedAlertId = _idConverterService.ConvertGuidToShortId(id, index);

			AwsS3ImageResponse uploadedImage = await _awsS3Client.UploadFoundAlertImageAsync(
				imageStream: compressedImage,
				imageFile: images[index],
				hashedAlertId);

			if (!uploadedImage.Success || uploadedImage.PublicUrl is null)
			{
				throw new InternalServerErrorException(
					"Não foi possível fazer upload da imagem, tente novamente mais tarde.");
			}

			uploadedImages.Add(uploadedImage.PublicUrl);
		}

		return uploadedImages;
	}
}