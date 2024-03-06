using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Infrastructure.ExternalServices.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices.AWS;

public class AwsS3Client : IAwsS3Client
{
	private readonly IAmazonS3 _s3Client;
	private readonly AwsData _awsData;
	private readonly ILogger<AwsS3Client> _logger;

	public AwsS3Client(
		IAmazonS3 s3Client,
		IOptions<AwsData> awsData,
		ILogger<AwsS3Client> logger)
	{
		_s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
		_awsData = awsData.Value ?? throw new ArgumentNullException(nameof(awsData));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<AwsS3ImageResponse> UploadPetImageAsync(
		MemoryStream imageStream, IFormFile imageFile, string hashedPetId)
	{
		try
		{
			PutObjectRequest putObjectRequest = new()
			{
				BucketName = _awsData.BucketName,
				// Content type and image extension is always set to webp, since
				// the image service always encodes the image as webp format
				Key = $"Images/{_awsData.PetImagesFolder}/{hashedPetId}.webp",
				ContentType = "image/webp",
				InputStream = imageStream,
				Metadata =
				{
					["x-amz-meta-originalname"] = imageFile.FileName,
					["x-amz-meta-extension"] = ".webp"
				}
			};
			await _s3Client.PutObjectAsync(putObjectRequest);

			return new AwsS3ImageResponse()
			{
				Success = true,
				PublicUrl = FormatPublicUrlString(putObjectRequest.Key),
			};
		}
		catch (AmazonS3Exception exception)
		{
			_logger.LogError(exception, exception.Message);
			return new AwsS3ImageResponse()
			{
				Success = false,
				PublicUrl = null
			};
		}
	}

	public async Task<AwsS3ImageResponse> UploadUserImageAsync(
		MemoryStream? imageStream, IFormFile? imageFile, string hashedUserId)
	{
		try
		{
			string awsImageKey = imageFile is null
				? _awsData.DefaultUserProfilePicture
				: $"Images/{_awsData.UserImagesFolder}/{hashedUserId}.webp";

			string fileNameMetadata = imageFile is null
				? _awsData.DefaultUserProfilePictureMetadata
				: imageFile.FileName;

			PutObjectRequest putObjectRequest = new()
			{
				BucketName = _awsData.BucketName,
				// Content type and image extension is always set to webp, since
				// the image service always encodes the image as webp format
				Key = awsImageKey,
				ContentType = "image/webp",
				InputStream = imageStream,
				Metadata =
				{
					["x-amz-meta-originalname"] = fileNameMetadata,
					["x-amz-meta-extension"] = ".webp"
				}
			};
			await _s3Client.PutObjectAsync(putObjectRequest);

			return new AwsS3ImageResponse()
			{
				Success = true,
				PublicUrl = FormatPublicUrlString(putObjectRequest.Key),
			};
		}
		catch (AmazonS3Exception exception)
		{
			_logger.LogError(exception, exception.Message);
			return new AwsS3ImageResponse()
			{
				Success = false,
				PublicUrl = null
			};
		}
	}

	public async Task<AwsS3ImageResponse> UploadFoundAlertImageAsync(
		MemoryStream imageStream, IFormFile imageFile, string hashedAlertId)
	{
		try
		{
			PutObjectRequest putObjectRequest = new()
			{
				BucketName = _awsData.BucketName,
				// Content type and image extension is always set to webp, since
				// the image service always encodes the image as webp format
				Key = $"Images/{_awsData.FoundAlertImagesFolder}/{hashedAlertId}.webp",
				ContentType = "image/webp",
				InputStream = imageStream,
				Metadata =
				{
					["x-amz-meta-originalname"] = imageFile.FileName,
					["x-amz-meta-extension"] = ".webp"
				}
			};
			await _s3Client.PutObjectAsync(putObjectRequest);

			return new AwsS3ImageResponse()
			{
				Success = true,
				PublicUrl = FormatPublicUrlString(putObjectRequest.Key),
			};
		}
		catch (AmazonS3Exception exception)
		{
			_logger.LogError(exception, exception.Message);
			return new AwsS3ImageResponse()
			{
				Success = false,
				PublicUrl = null
			};
		}
	}

	public async Task<AwsS3ImageResponse> DeletePetImageAsync(string hashedPetId)
	{
		try
		{
			DeleteObjectRequest deleteObjectRequest = new()
			{
				BucketName = _awsData.BucketName,
				Key = $"Images/{_awsData.PetImagesFolder}/{hashedPetId}.webp",
			};
			await _s3Client.DeleteObjectAsync(deleteObjectRequest);

			return new AwsS3ImageResponse()
			{
				Success = true,
				PublicUrl = null
			};
		}
		catch (AmazonS3Exception exception)
		{
			_logger.LogError(exception, exception.Message);
			return new AwsS3ImageResponse()
			{
				Success = false,
				PublicUrl = null
			};
		}
	}

	public async Task<AwsS3ImageResponse> DeleteFoundAlertImageAsync(string hashedAlertId)
	{
		try
		{
			DeleteObjectRequest deleteObjectRequest = new()
			{
				BucketName = _awsData.BucketName,
				Key = $"Images/{_awsData.FoundAlertImagesFolder}/{hashedAlertId}.webp",
			};
			await _s3Client.DeleteObjectAsync(deleteObjectRequest);

			return new AwsS3ImageResponse()
			{
				Success = true,
				PublicUrl = null
			};
		}
		catch (AmazonS3Exception exception)
		{
			_logger.LogError(exception, exception.Message);
			return new AwsS3ImageResponse()
			{
				Success = false,
				PublicUrl = null
			};
		}
	}

	private string FormatPublicUrlString(string imageKey)
	{
		return $"https://{_awsData.BucketName}.s3.{_awsData.Region}.amazonaws.com/{imageKey}";
	}
}