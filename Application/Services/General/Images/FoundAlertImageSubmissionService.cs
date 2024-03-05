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

	public async Task<string> UploadFoundAlertImageAsync(Guid foundAlertId, IFormFile alertImage)
	{
		MemoryStream compressedImage = await _imageProcessingService.CompressImageAsync(alertImage.OpenReadStream());

		string hashedAlertId = _idConverterService.ConvertGuidToShortId(foundAlertId);

		AwsS3ImageResponse uploadedImage = await _awsS3Client.UploadUserImageAsync(
			imageStream: compressedImage,
			imageFile: alertImage,
			hashedAlertId);

		if (!uploadedImage.Success || uploadedImage.PublicUrl is null)
		{
			throw new InternalServerErrorException(
				"Não foi possível fazer upload da imagem, tente novamente mais tarde.");
		}

		return uploadedImage.PublicUrl;
	}
}