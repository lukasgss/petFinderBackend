using Application.Common.Interfaces.FrontendDropdownData;

namespace Application.Common.Interfaces.Entities.Breeds;

public interface IBreedService
{
    Task<IEnumerable<DropdownDataResponse<int>>> GetBreedsForDropdown(string breedName, int speciesId);
}