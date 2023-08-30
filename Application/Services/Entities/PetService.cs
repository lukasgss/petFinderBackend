using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users;
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
    private readonly IUserRepository _userRepository;

    public PetService(IPetRepository petRepository,
        IBreedRepository breedRepository,
        ISpeciesRepository speciesRepository,
        IColorRepository colorRepository,
        IGuidProvider guidProvider,
        IUserRepository userRepository)
    {
        _petRepository = petRepository ?? throw new ArgumentNullException(nameof(petRepository));
        _breedRepository = breedRepository ?? throw new ArgumentNullException(nameof(breedRepository));
        _speciesRepository = speciesRepository ?? throw new ArgumentNullException(nameof(speciesRepository));
        _colorRepository = colorRepository ?? throw new ArgumentNullException(nameof(colorRepository));
        _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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
    
    public async Task<PetResponse> CreatePetAsync(CreatePetRequest createPetRequest, Guid? userId)
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

        User? petOwner = null;
        if (userId is not null)
        {
            petOwner = await _userRepository.GetUserByIdAsync((Guid)userId);
        }

        Pet petToBeCreated = new()
        {
            Id = _guidProvider.NewGuid(),
            Name = createPetRequest.Name,
            Observations = createPetRequest.Observations,
            Owner = petOwner,
            Breed = breed,
            Species = species,
            Colors = colors
        };

        _petRepository.Add(petToBeCreated);
        await _petRepository.CommitAsync();

        return petToBeCreated.ConvertToPetResponse(
            owner: petOwner?.ConvertToOwnerResponse(),
            colors: colors.ConvertToListOfColorResponse(),
            breed.ConvertToBreedResponse());
    }
}