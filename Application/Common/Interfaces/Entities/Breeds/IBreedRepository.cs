using Application.Common.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Common.Interfaces.Entities.Breeds;

public interface IBreedRepository : IGenericRepository<Breed>
{
	Task<Breed?> GetBreedByIdAsync(int breedId);
	Task<List<Breed>> GetMultipleBreedsByIdAsync(IEnumerable<int> breedIds);
	Task<IEnumerable<Breed>> GetBreedsBySpeciesIdAsync(int speciesId);
	Task<IEnumerable<Breed>> GetBreedsByNameAsync(string breedName, int speciesId);
}