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

        return searchedPet.ToPetResponse(searchedPet.Owner, searchedPet.Colors, searchedPet.Breed);
    }

    public async Task<PetResponse> CreatePetAsync(CreatePetRequest createPetRequest, Guid? userId)
    {
        Breed breed = await GetBreedAsync(createPetRequest.BreedId);
        Species species = await GetSpeciesAsync(createPetRequest.SpeciesId);
        List<Color> colors = await GetColorsAsync(createPetRequest.ColorIds);

        User? petOwner = await AssignPetOwner(userId);

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

        return petToBeCreated.ToPetResponse(petOwner, colors, breed);
    }

    public async Task<PetResponse> EditPetAsync(EditPetRequest editPetRequest, Guid? userId, Guid routeId)
    {
        if (editPetRequest.Id != routeId)
        {
            throw new BadRequestException("Id da rota não coincide com o id especificado.");
        }
        Pet dbPet = await GetPetAsync(editPetRequest.Id);

        Breed breed = await GetBreedAsync(editPetRequest.BreedId);
        Species species = await GetSpeciesAsync(editPetRequest.SpeciesId);
        List<Color> colors = await GetColorsAsync(editPetRequest.ColorIds);
        User? petOwner = await AssignPetOwner(userId);

        if (petOwner is null || dbPet.Owner?.Id != petOwner.Id)
        {
            throw new UnauthorizedException("Você não possui permissão para editar dados desse animal.");
        }

        dbPet.Id = editPetRequest.Id;
        dbPet.Name = editPetRequest.Name;
        dbPet.Observations= editPetRequest.Observations;
        dbPet.Owner = petOwner;
        dbPet.Breed = breed;
        dbPet.Colors = colors;
        dbPet.Species = species;
        
        await _petRepository.CommitAsync();

        return dbPet.ToPetResponse(petOwner, colors, breed);
    }

    public async Task DeletePetAsync(Guid petId, Guid? userId)
    {
        Pet petToDelete = await GetPetAsync(petId);
        if (petToDelete.Owner is null || petToDelete.Owner.Id != userId)
        {
            throw new UnauthorizedException("Você não possui permissão para excluir o animal.");
        }
        
        _petRepository.Delete(petToDelete);
        await _petRepository.CommitAsync();
    }
    
    private async Task<User?> AssignPetOwner(Guid? userId)
    {
        if (userId is null)
        {
            return null;
        }

        return await _userRepository.GetUserByIdAsync((Guid)userId);
    }

    private async Task<Pet> GetPetAsync(Guid petId)
    {
        Pet? pet = await _petRepository.GetPetByIdAsync(petId);
        if (pet is null)
        {
            throw new NotFoundException("Animal especificado não existe.");
        }

        return pet;
    }

    private async Task<Breed> GetBreedAsync(int breedId)
    {
        Breed? breed = await _breedRepository.GetBreedByIdAsync(breedId);
        if (breed is null)
        {
            throw new NotFoundException("Raça especificada não existe.");
        }

        return breed;
    }

    private async Task<Species> GetSpeciesAsync(int speciesId)
    {
        Species? species = await _speciesRepository.GetSpeciesByIdAsync(speciesId);
        if (species is null)
        {
            throw new NotFoundException("Espécie especificada não existe.");
        }

        return species;
    }

    private async Task<List<Color>> GetColorsAsync(List<int> colorIds)
    {
        List<Color> colors = await _colorRepository.GetMultipleColorsByIdsAsync(colorIds);
        if (colors.Count != colorIds.Count || colors.Count == 0)
        {
            throw new NotFoundException("Alguma das cores especificadas não existe.");
        }

        return colors;
    }
}