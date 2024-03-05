using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.General.Images;

public interface IPetImageSubmissionService
{
	Task<List<string>> UploadPetImageAsync(Guid petId, List<IFormFile> petImages);
	Task<List<string>> UpdatePetImageAsync(Guid petId, List<IFormFile> newlyAddedImages, int previousImageCount);
	Task DeletePetImageAsync(Guid petId, List<PetImage> petImages);
}