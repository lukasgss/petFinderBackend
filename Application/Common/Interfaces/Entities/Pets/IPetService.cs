using Application.Common.Interfaces.Entities.Pets.DTOs;

namespace Application.Common.Interfaces.Entities.Pets;

public interface IPetService
{
    Task<PetResponse> GetPetBydIdAsync(Guid petId);
    Task<PetResponse> CreatePetAsync(CreatePetRequest createPetRequest);
}