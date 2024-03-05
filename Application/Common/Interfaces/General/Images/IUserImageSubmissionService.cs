using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.General.Images;

public interface IUserImageSubmissionService
{
	Task<string> UploadUserImageAsync(Guid userId, IFormFile? userImage);
}