using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Services.General.Images;

public class FoundAlertImageSubmissionService : IFoundAlertImageSubmissionService
{
	private readonly IImageProcessingService _imageProcessingService;
	private readonly IFileUploadClient _fileUploadClient;
	private readonly IIdConverterService _idConverterService;
	private readonly ILogger<FoundAlertImageSubmissionService> _logger;

	public FoundAlertImageSubmissionService(
		IImageProcessingService imageProcessingService,
		IFileUploadClient fileUploadClient,
		IIdConverterService idConverterService,
		ILogger<FoundAlertImageSubmissionService> logger)
	{
		_imageProcessingService =
			imageProcessingService ?? throw new ArgumentNullException(nameof(imageProcessingService));
		_fileUploadClient = fileUploadClient ?? throw new ArgumentNullException(nameof(fileUploadClient));
		_idConverterService = idConverterService ?? throw new ArgumentNullException(nameof(idConverterService));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

			AwsS3ImageResponse response = await _fileUploadClient.DeleteFoundAlertImageAsync(hashedId);
			if (!response.Success)
			{
				_logger.LogInformation("Não foi possível excluir a imagem {ImageId}", hashedId);
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

			AwsS3ImageResponse uploadedImage = await _fileUploadClient.UploadFoundAlertImageAsync(
				imageStream: compressedImage,
				imageFile: images[index],
				hashedAlertId);

			if (!uploadedImage.Success || uploadedImage.PublicUrl is null)
			{
				_logger.LogInformation("Não foi possível inserir a imagem {ImageId}", hashedAlertId);
				throw new InternalServerErrorException(
					"Não foi possível fazer upload da imagem, tente novamente mais tarde.");
			}

			uploadedImages.Add(uploadedImage.PublicUrl);
		}

		return uploadedImages;
	}
}