using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Services.Entities;

public class BreedService : IBreedService
{
    private readonly IBreedRepository _breedRepository;

    public BreedService(IBreedRepository breedRepository)
    {
        _breedRepository = breedRepository ?? throw new ArgumentNullException(nameof(breedRepository));
    }

    public async Task<IEnumerable<DropdownDataResponse<int>>> GetBreedsForDropdown(string breedName, int speciesId)
    {
        if (breedName.Length < 2)
        {
            throw new BadRequestException("Preencha no mínimo 2 caracteres para buscar a raça pelo nome.");
        }

        IEnumerable<Breed> breeds = await _breedRepository.GetBreedsByNameAsync(breedName, speciesId);
        
        return breeds.ConvertToListOfDropdownData();
    }
}