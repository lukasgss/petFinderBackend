using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Services.General.Images;

public class UserImageSubmissionService : IUserImageSubmissionService
{
	private readonly IImageProcessingService _imageProcessingService;
	private readonly IFileUploadClient _fileUploadClient;
	private readonly IIdConverterService _idConverterService;
	private readonly ImagesData _imagesData;

	public UserImageSubmissionService(
		IImageProcessingService imageProcessingService,
		IFileUploadClient fileUploadClient,
		IIdConverterService idConverterService,
		IOptions<ImagesData> imagesData)
	{
		_imageProcessingService =
			imageProcessingService ?? throw new ArgumentNullException(nameof(imageProcessingService));
		_fileUploadClient = fileUploadClient ?? throw new ArgumentNullException(nameof(fileUploadClient));
		_idConverterService = idConverterService ?? throw new ArgumentNullException(nameof(idConverterService));
		_imagesData = imagesData.Value ?? throw new ArgumentNullException(nameof(imagesData));
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

	public string ValidateUserImage(string userImage)
	{
		const int maxLengthOfImageUrl = 180;

		return userImage.Length > maxLengthOfImageUrl ? _fileUploadClient.FormatPublicUrlString(null) : userImage;
	}
}