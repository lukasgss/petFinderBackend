using Application.Common.Interfaces.FrontendDropdownData;

namespace Application.Common.Interfaces.Entities.Breeds;

public interface IBreedService
{
    Task<List<DropdownDataResponse<int>>> GetBreedsForDropdown(string breedName, int speciesId);
}