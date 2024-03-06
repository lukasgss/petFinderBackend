using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.General.Images;

public interface IFoundAlertImageSubmissionService
{
	Task<IReadOnlyList<string>> UploadImagesAsync(Guid alertId, List<IFormFile> alertImages);
	Task<IReadOnlyList<string>> UpdateImagesAsync(Guid petId, List<IFormFile> newlyAddedImages, int previousImageCount);
}