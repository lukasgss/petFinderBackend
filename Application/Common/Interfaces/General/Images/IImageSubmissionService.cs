using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.General.Images;

public interface IImageSubmissionService
{
	Task<List<string>> UploadPetImageAsync(Guid petId, List<IFormFile> petImages);
	Task<string> UploadUserImageAsync(Guid userId, IFormFile? userImage);
	Task<string> UploadFoundAlertImageAsync(Guid foundAlertId, IFormFile alertImage);
	Task DeletePetImageAsync(Guid petId);
}