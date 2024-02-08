using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.General.Images;

public interface IImageSubmissionService
{
    Task<string> UploadPetImageAsync(Guid petId, IFormFile petImage);
    Task<string> UploadUserImageAsync(Guid userId, IFormFile? userImage);
    Task<string>UploadFoundAlertImageAsync(Guid foundAlertId, IFormFile alertImage);
    Task DeletePetImageAsync(Guid petId);
}