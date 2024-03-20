using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Images;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Services.General.Images;

public class PetImageSubmissionService : IPetImageSubmissionService
{
	private readonly IImageProcessingService _imageProcessingService;
	private readonly IFileUploadClient _fileUploadClient;
	private readonly IIdConverterService _idConverterService;
	private readonly ILogger<PetImageSubmissionService> _logger;

	public PetImageSubmissionService(
		IImageProcessingService imageProcessingService,
		IFileUploadClient fileUploadClient,
		IIdConverterService idConverterService,
		ILogger<PetImageSubmissionService> logger)
	{
		_imageProcessingService =
			imageProcessingService ?? throw new ArgumentNullException(nameof(imageProcessingService));
		_fileUploadClient = fileUploadClient ?? throw new ArgumentNullException(nameof(fileUploadClient));
		_idConverterService = idConverterService ?? throw new ArgumentNullException(nameof(idConverterService));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<List<string>> UploadPetImageAsync(Guid petId, List<IFormFile> petImages)
	{
		return await UploadImages(petId, petImages);
	}

	public async Task<List<string>> UpdatePetImageAsync(
		Guid petId, List<IFormFile> newlyAddedImages, int previousImageCount)
	{
		List<string> uploadedImages = await UploadImages(petId, newlyAddedImages);

		if (uploadedImages.Count < previousImageCount)
		{
			await DeletePreviousPetImagesAsync(petId, uploadedImages.Count, previousImageCount);
		}

		return uploadedImages;
	}

	public async Task DeletePetImageAsync(Guid petId, List<PetImage> petImages)
	{
		for (int index = 0; index < petImages.Count; index++)
		{
			string hashedPetId = _idConverterService.ConvertGuidToShortId(petId, index);

			AwsS3ImageResponse response = await _fileUploadClient.DeletePetImageAsync(hashedPetId);
			if (!response.Success)
			{
				_logger.LogInformation("Não foi possível excluir a imagem {ImageId}", hashedPetId);
				throw new InternalServerErrorException(
					"Não foi possível excluir a imagem do animal, tente novamente mais tarde.");
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

			string hashedPetId = _idConverterService.ConvertGuidToShortId(id, index);

			AwsS3ImageResponse uploadedImage = await _fileUploadClient.UploadPetImageAsync(
				imageStream: compressedImage,
				imageFile: images[index],
				hashedPetId);

			if (!uploadedImage.Success || uploadedImage.PublicUrl is null)
			{
				_logger.LogInformation("Não foi possível inserir a imagem {ImageId}", hashedPetId);
				throw new InternalServerErrorException(
					"Não foi possível fazer upload da imagem, tente novamente mais tarde.");
			}

			uploadedImages.Add(uploadedImage.PublicUrl);
		}

		return uploadedImages;
	}

	private async Task DeletePreviousPetImagesAsync(Guid id, int currentImageCount, int previousImageCount)
	{
		for (int index = currentImageCount; index < previousImageCount; index++)
		{
			string hashedId = _idConverterService.ConvertGuidToShortId(id, index);

			AwsS3ImageResponse response = await _fileUploadClient.DeletePetImageAsync(hashedId);
			if (!response.Success)
			{
				_logger.LogInformation("Não foi possível excluir a imagem {ImageId}", hashedId);
				throw new InternalServerErrorException(
					"Não foi possível excluir a imagem, tente novamente mais tarde.");
			}
		}
	}
}