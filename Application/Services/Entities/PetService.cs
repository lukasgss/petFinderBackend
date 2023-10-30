using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Images;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Color = Domain.Entities.Color;

namespace Application.Services.Entities;

public class PetService : IPetService
{
    private readonly IPetRepository _petRepository;
    private readonly IBreedRepository _breedRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IColorRepository _colorRepository;
    private readonly IGuidProvider _guidProvider;
    private readonly IUserRepository _userRepository;
    private readonly IAwsS3Client _awsS3Client;
    private readonly IImageService _imageService;

    public PetService(
        IPetRepository petRepository,
        IBreedRepository breedRepository,
        ISpeciesRepository speciesRepository,
        IColorRepository colorRepository,
        IGuidProvider guidProvider,
        IUserRepository userRepository,
        IAwsS3Client awsS3Client,
        IImageService imageService)
    {
        _petRepository = petRepository ?? throw new ArgumentNullException(nameof(petRepository));
        _breedRepository = breedRepository ?? throw new ArgumentNullException(nameof(breedRepository));
        _speciesRepository = speciesRepository ?? throw new ArgumentNullException(nameof(speciesRepository));
        _colorRepository = colorRepository ?? throw new ArgumentNullException(nameof(colorRepository));
        _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _awsS3Client = awsS3Client ?? throw new ArgumentNullException(nameof(awsS3Client));
        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    }

    public async Task<PetResponse> GetPetBydIdAsync(Guid petId)
    {
        Pet searchedPet = await ValidateAndAssignPetAsync(petId);

        return searchedPet.ToPetResponse(searchedPet.Owner, searchedPet.Colors, searchedPet.Breed);
    }

    public async Task<PetResponse> CreatePetAsync(CreatePetRequest createPetRequest, Guid userId)
    {
        Breed breed = await ValidateAndAssignBreedAsync(createPetRequest.BreedId);
        Species species = await ValidateAndAssignSpeciesAsync(createPetRequest.SpeciesId);
        List<Color> colors = await ValidateAndAssignColorsAsync(createPetRequest.ColorIds);
        User petOwner = await ValidateAndAssignUserAsync(userId);

        Guid petId = _guidProvider.NewGuid();

        await using MemoryStream compressedImage =
            await _imageService.CompressImageAsync(createPetRequest.Image.OpenReadStream());

        AwsS3ImageResponse uploadedImage = await _awsS3Client.UploadPetImageAsync(
            imageStream: compressedImage,
            imageFile: createPetRequest.Image,
            petId);

        if (!uploadedImage.Success || uploadedImage.PublicUrl is null)
        {
            throw new InternalServerErrorException(
                "Não foi possível fazer upload da imagem, tente novamente mais tarde.");
        }

        Pet petToBeCreated = new()
        {
            Id = petId,
            Name = createPetRequest.Name,
            Observations = createPetRequest.Observations,
            Gender = createPetRequest.Gender,
            AgeInMonths = createPetRequest.AgeInMonths,
            Image = uploadedImage.PublicUrl,
            Owner = petOwner,
            Breed = breed,
            Species = species,
            Colors = colors
        };
        _petRepository.Add(petToBeCreated);
        await _petRepository.CommitAsync();

        return petToBeCreated.ToPetResponse(petOwner, colors, breed);
    }

    public async Task<PetResponse> EditPetAsync(EditPetRequest editPetRequest, Guid userId, Guid routeId)
    {
        if (editPetRequest.Id != routeId)
        {
            throw new BadRequestException("Id da rota não coincide com o id especificado.");
        }

        Pet dbPet = await ValidateAndAssignPetAsync(editPetRequest.Id);

        Breed breed = await ValidateAndAssignBreedAsync(editPetRequest.BreedId);
        Species species = await ValidateAndAssignSpeciesAsync(editPetRequest.SpeciesId);
        List<Color> colors = await ValidateAndAssignColorsAsync(editPetRequest.ColorIds);
        User petOwner = await ValidateAndAssignUserAsync(userId);

        if (dbPet.Owner.Id != userId)
        {
            throw new UnauthorizedException("Você não possui permissão para editar dados desse animal.");
        }

        await using MemoryStream compressedImage =
            await _imageService.CompressImageAsync(editPetRequest.Image.OpenReadStream());

        AwsS3ImageResponse uploadedImage = await _awsS3Client.UploadPetImageAsync(
            imageStream: compressedImage,
            imageFile: editPetRequest.Image,
            dbPet.Id);

        if (!uploadedImage.Success || uploadedImage.PublicUrl is null)
        {
            throw new InternalServerErrorException(
                "Não foi possível fazer upload da imagem, tente novamente mais tarde.");
        }

        dbPet.Id = editPetRequest.Id;
        dbPet.Name = editPetRequest.Name;
        dbPet.Observations = editPetRequest.Observations;
        dbPet.Gender = editPetRequest.Gender;
        dbPet.AgeInMonths = editPetRequest.AgeInMonths;
        dbPet.Image = uploadedImage.PublicUrl;
        dbPet.Owner = petOwner;
        dbPet.Breed = breed;
        dbPet.Colors = colors;
        dbPet.Species = species;

        await _petRepository.CommitAsync();

        return dbPet.ToPetResponse(petOwner, colors, breed);
    }

    public async Task DeletePetAsync(Guid petId, Guid userId)
    {
        Pet petToDelete = await ValidateAndAssignPetAsync(petId);
        if (petToDelete.Owner.Id != userId)
        {
            throw new UnauthorizedException("Você não possui permissão para excluir o animal.");
        }

        AwsS3ImageResponse response = await _awsS3Client.DeletePetImageAsync(petToDelete.Id);
        if (!response.Success)
        {
            throw new InternalServerErrorException("Não foi possível excluir o animal, tente novamente mais tarde.");
        }

        _petRepository.Delete(petToDelete);
        await _petRepository.CommitAsync();
    }

    private async Task<User> ValidateAndAssignUserAsync(Guid userId)
    {
        User? user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("Usuário com o id especificado não existe.");
        }

        return user;
    }

    private async Task<Pet> ValidateAndAssignPetAsync(Guid petId)
    {
        Pet? pet = await _petRepository.GetPetByIdAsync(petId);
        if (pet is null)
        {
            throw new NotFoundException("Animal com o id especificado não existe.");
        }

        return pet;
    }

    private async Task<Breed> ValidateAndAssignBreedAsync(int breedId)
    {
        Breed? breed = await _breedRepository.GetBreedByIdAsync(breedId);
        if (breed is null)
        {
            throw new NotFoundException("Raça especificada não existe.");
        }

        return breed;
    }

    private async Task<Species> ValidateAndAssignSpeciesAsync(int speciesId)
    {
        Species? species = await _speciesRepository.GetSpeciesByIdAsync(speciesId);
        if (species is null)
        {
            throw new NotFoundException("Espécie especificada não existe.");
        }

        return species;
    }

    private async Task<List<Color>> ValidateAndAssignColorsAsync(List<int> colorIds)
    {
        List<Color> colors = await _colorRepository.GetMultipleColorsByIdsAsync(colorIds);
        if (colors.Count != colorIds.Count || colors.Count == 0)
        {
            throw new NotFoundException("Alguma das cores especificadas não existe.");
        }

        return colors;
    }
}