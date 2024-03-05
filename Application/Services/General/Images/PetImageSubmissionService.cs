using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Images;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Application.Services.General.Images;

public class PetImageSubmissionService : IPetImageSubmissionService
{
	private readonly IImageProcessingService _imageProcessingService;
	private readonly IAwsS3Client _awsS3Client;
	private readonly IIdConverterService _idConverterService;

	public PetImageSubmissionService(
		IImageProcessingService imageProcessingService,
		IAwsS3Client awsS3Client,
		IIdConverterService idConverterService)
	{
		_imageProcessingService =
			imageProcessingService ?? throw new ArgumentNullException(nameof(imageProcessingService));
		_awsS3Client = awsS3Client ?? throw new ArgumentNullException(nameof(awsS3Client));
		_idConverterService = idConverterService ?? throw new ArgumentNullException(nameof(idConverterService));
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

			AwsS3ImageResponse response = await _awsS3Client.DeletePetImageAsync(hashedPetId);
			if (!response.Success)
			{
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

			AwsS3ImageResponse uploadedImage = await _awsS3Client.UploadPetImageAsync(
				imageStream: compressedImage,
				imageFile: images[index],
				hashedPetId);

			if (!uploadedImage.Success || uploadedImage.PublicUrl is null)
			{
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

			AwsS3ImageResponse response = await _awsS3Client.DeletePetImageAsync(hashedId);
			if (!response.Success)
			{
				throw new InternalServerErrorException(
					"Não foi possível excluir a imagem, tente novamente mais tarde.");
			}
		}
	}
}