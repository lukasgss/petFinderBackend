using Application.Common.Interfaces.GenericRepository;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Common.Interfaces.Entities.Pets;

public interface IPetRepository : IGenericRepository<Pet>
{
	Task<Pet?> GetPetByIdAsync(Guid petId);
	Task<bool> CreatePetAndImages(Pet petToBeCreated, List<PetImage> imageUrls);
}