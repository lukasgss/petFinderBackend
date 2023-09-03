using Application.Common.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Common.Interfaces.Entities.AnimalSpecies;

public interface ISpeciesRepository : IGenericRepository<Species>
{
    Task<IEnumerable<Species>> GetAllSpecies();
    Task<Species?> GetSpeciesByIdAsync(int speciesId);
}