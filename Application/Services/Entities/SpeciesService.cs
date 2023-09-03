using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Services.Entities;

public class SpeciesService : ISpeciesService
{
    private readonly ISpeciesRepository _speciesRepository;

    public SpeciesService(ISpeciesRepository speciesRepository)
    {
        _speciesRepository = speciesRepository ?? throw new ArgumentNullException(nameof(speciesRepository));
    }

    public async Task<IEnumerable<DropdownDataResponse<int>>> GetAllSpeciesForDropdown()
    {
        IEnumerable<Species> species = await _speciesRepository.GetAllSpecies();

        return species.ConvertToListOfDropdownData();
    }
}