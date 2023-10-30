using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Interfaces.Converters;
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
    private readonly IIdConverterService _idConverterService;
    private readonly ILogger<AwsS3Client> _logger;

    public AwsS3Client(
        IAmazonS3 s3Client,
        IOptions<AwsData> awsData,
        IIdConverterService idConverterService,
        ILogger<AwsS3Client> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
        _idConverterService = idConverterService;
        _awsData = awsData.Value;
    }

    public async Task<AwsS3ImageResponse> UploadPetImageAsync(MemoryStream imageStream, IFormFile imageFile, Guid petId)
    {
        try
        {
            string hashedPetId = _idConverterService.ConvertGuidToShortId(petId);

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

    public async Task<AwsS3ImageResponse> DeletePetImageAsync(Guid petId)
    {
        try
        {
            string hashedPetId = _idConverterService.ConvertGuidToShortId(petId);

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

    private string FormatPublicUrlString(string imageKey)
    {
        return $"https://{_awsData.BucketName}.s3.{_awsData.Region}.amazonaws.com/{imageKey}";
    }
}