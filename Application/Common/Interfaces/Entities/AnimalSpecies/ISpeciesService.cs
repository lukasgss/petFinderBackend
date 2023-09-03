using Application.Common.Interfaces.FrontendDropdownData;

namespace Application.Common.Interfaces.Entities.AnimalSpecies;

public interface ISpeciesService
{
    Task<IEnumerable<DropdownDataResponse<int>>> GetAllSpeciesForDropdown();
}