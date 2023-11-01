using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.ExternalServices.AWS;

public interface IAwsS3Client
{
    Task<AwsS3ImageResponse> UploadPetImageAsync(MemoryStream imageStream, IFormFile imageFile, Guid petId);
    Task<AwsS3ImageResponse> UploadUserImageAsync(MemoryStream imageStream, IFormFile imageFile, Guid userId);
    Task<AwsS3ImageResponse> DeletePetImageAsync(Guid petId);
}