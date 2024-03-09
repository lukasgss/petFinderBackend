using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Images;
using Microsoft.AspNetCore.Http;

namespace Application.Services.General.Images;

public class UserImageSubmissionService : IUserImageSubmissionService
{
	private readonly IImageProcessingService _imageProcessingService;
	private readonly IFileUploadClient _fileUploadClient;
	private readonly IIdConverterService _idConverterService;

	public UserImageSubmissionService(
		IImageProcessingService imageProcessingService,
		IFileUploadClient fileUploadClient,
		IIdConverterService idConverterService)
	{
		_imageProcessingService =
			imageProcessingService ?? throw new ArgumentNullException(nameof(imageProcessingService));
		_fileUploadClient = fileUploadClient ?? throw new ArgumentNullException(nameof(fileUploadClient));
		_idConverterService = idConverterService ?? throw new ArgumentNullException(nameof(idConverterService));
	}

	public async Task<string> UploadUserImageAsync(Guid userId, IFormFile? userImage)
	{
		await using MemoryStream? compressedImage = userImage is not null
			? await _imageProcessingService.CompressImageAsync(userImage.OpenReadStream())
			: null;

		string hashedUserId = _idConverterService.ConvertGuidToShortId(userId);

		AwsS3ImageResponse uploadedImage = await _fileUploadClient.UploadUserImageAsync(
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
}