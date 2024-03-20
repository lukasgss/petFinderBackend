using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.General.Images;

public class UserImageSubmissionService : IUserImageSubmissionService
{
	private readonly IImageProcessingService _imageProcessingService;
	private readonly IFileUploadClient _fileUploadClient;
	private readonly IIdConverterService _idConverterService;
	private readonly ILogger<UserImageSubmissionService> _logger;

	public UserImageSubmissionService(
		IImageProcessingService imageProcessingService,
		IFileUploadClient fileUploadClient,
		IIdConverterService idConverterService,
		ILogger<UserImageSubmissionService> logger)
	{
		_imageProcessingService =
			imageProcessingService ?? throw new ArgumentNullException(nameof(imageProcessingService));
		_fileUploadClient = fileUploadClient ?? throw new ArgumentNullException(nameof(fileUploadClient));
		_idConverterService = idConverterService ?? throw new ArgumentNullException(nameof(idConverterService));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
			_logger.LogInformation("Não foi possível inserir a imagem {ImageId}", hashedUserId);
			throw new InternalServerErrorException(
				"Não foi possível fazer upload da imagem, tente novamente mais tarde.");
		}

		return uploadedImage.PublicUrl;
	}

	public string ValidateUserImage(string userImage)
	{
		const int maxLengthOfImageUrl = 180;

		return userImage.Length > maxLengthOfImageUrl ? _fileUploadClient.FormatPublicUrlString(null) : userImage;
	}
}