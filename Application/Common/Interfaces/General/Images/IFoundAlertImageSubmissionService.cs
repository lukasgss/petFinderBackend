using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.General.Images;

public interface IFoundAlertImageSubmissionService
{
	Task<string> UploadFoundAlertImageAsync(Guid foundAlertId, IFormFile alertImage);
}