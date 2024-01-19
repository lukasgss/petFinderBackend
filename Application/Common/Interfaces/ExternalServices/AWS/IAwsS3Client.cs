using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.ExternalServices.AWS;

public interface IAwsS3Client
{
	Task<AwsS3ImageResponse> UploadPetImageAsync(MemoryStream imageStream, IFormFile imageFile, string hashedPetId);
	Task<AwsS3ImageResponse> UploadUserImageAsync(MemoryStream? imageStream, IFormFile? imageFile, string hashedUserId);
	Task<AwsS3ImageResponse> DeletePetImageAsync(string hashedPetId);
}