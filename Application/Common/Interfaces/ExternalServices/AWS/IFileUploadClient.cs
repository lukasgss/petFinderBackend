using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.ExternalServices.AWS;

public interface IFileUploadClient
{
	Task<AwsS3ImageResponse> UploadPetImageAsync(MemoryStream imageStream, IFormFile imageFile, string hashedPetId);
	Task<AwsS3ImageResponse> UploadUserImageAsync(MemoryStream? imageStream, IFormFile? imageFile, string hashedUserId);

	Task<AwsS3ImageResponse> UploadFoundAlertImageAsync(
		MemoryStream imageStream, IFormFile imageFile, string hashedAlertId);

	Task<AwsS3ImageResponse> DeletePetImageAsync(string hashedPetId);
	Task<AwsS3ImageResponse> DeleteFoundAlertImageAsync(string hashedAlertId);
	string FormatPublicUrlString(string? imageKey);
}