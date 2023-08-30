using Application.Common.Interfaces.Entities.Pets.DTOs;

namespace Application.Common.Interfaces.Entities.Pets;

public interface IPetService
{
    Task<PetResponse> GetPetBydIdAsync(Guid petId);
    Task<PetResponse> CreatePetAsync(CreatePetRequest createPetRequest, Guid? userId);
    Task<PetResponse> EditPetAsync(EditPetRequest editPetRequest, Guid? userId, Guid routeId);
    Task DeletePetAsync(Guid petId, Guid? userId);
}