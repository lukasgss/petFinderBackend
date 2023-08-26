using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Providers;
using Domain.Entities;

namespace Application.Services.Entities;

public class PetService : IPetService
{
    private readonly IPetRepository _petRepository;
    private readonly IBreedRepository _breedRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IColorRepository _colorRepository;
    private readonly IGuidProvider _guidProvider;

    public PetService(IPetRepository petRepository,
        IBreedRepository breedRepository,
        ISpeciesRepository speciesRepository,
        IColorRepository colorRepository,
        IGuidProvider guidProvider)
    {
        _petRepository = petRepository;
        _breedRepository = breedRepository;
        _speciesRepository = speciesRepository;
        _colorRepository = colorRepository;
        _guidProvider = guidProvider;
    }

    public async Task<PetResponse> CreatePetAsync(CreatePetRequest createPetRequest)
    {
        Breed? breed = await _breedRepository.GetBreedByIdAsync(createPetRequest.BreedId);
        if (breed is null)
        {
            throw new NotFoundException("Raça especificada não existe.");
        }

        Species? species = await _speciesRepository.GetSpeciesByIdAsync(createPetRequest.SpeciesId);
        if (species is null)
        {
            throw new NotFoundException("Espécie especificada não existe.");
        }

        List<Color> colors = await _colorRepository.GetMultipleColorsByIdsAsync(createPetRequest.ColorIds);
        if (colors.Count != createPetRequest.ColorIds.Count || colors.Count == 0)
        {
            throw new NotFoundException("Alguma das cores especificadas não existe.");
        }

        // TODO: Once registration is made, check if user is logged in.
        // if he is, get his user id and register him as the owner of the pet

        Pet petToBeCreated = new()
        {
            Id = _guidProvider.NewGuid(),
            Name = createPetRequest.Name,
            Observations = createPetRequest.Observations,
            Owner = null,
            Breed = breed,
            Species = species,
            Colors = colors
        };

        _petRepository.Add(petToBeCreated);
        await _petRepository.CommitAsync();

        return petToBeCreated.ConvertToPetResponse(
            owner: null,
            colors: colors.ConvertToListOfColorResponse(),
            breed.ConvertToBreedResponse());
    }

    public async Task<PetResponse> GetPetBydIdAsync(Guid petId)
    {
        Pet? searchedPet = await _petRepository.GetPetByIdAsync(petId);
        if (searchedPet is null)
        {
            throw new NotFoundException("Animal com o id especificado não existe.");
        }

        return new PetResponse()
        {
            Id = searchedPet.Id,
            Name = searchedPet.Name,
            Observations = searchedPet.Observations,
            Owner = searchedPet.Owner?.ConvertToOwnerResponse(),
            Breed = searchedPet.Breed.ConvertToBreedResponse(),
            Colors = searchedPet.Colors.ConvertToListOfColorResponse()
        };
    }
}