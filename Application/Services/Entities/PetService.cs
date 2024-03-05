using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Vaccines;
using Application.Common.Interfaces.General.Images;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Color = Domain.Entities.Color;

namespace Application.Services.Entities;

public class PetService : IPetService
{
	private readonly IPetRepository _petRepository;
	private readonly IBreedRepository _breedRepository;
	private readonly ISpeciesRepository _speciesRepository;
	private readonly IColorRepository _colorRepository;
	private readonly IUserRepository _userRepository;
	private readonly IVaccineRepository _vaccineRepository;
	private readonly IPetImageSubmissionService _imageSubmissionService;
	private readonly IValueProvider _valueProvider;

	public PetService(
		IPetRepository petRepository,
		IBreedRepository breedRepository,
		ISpeciesRepository speciesRepository,
		IColorRepository colorRepository,
		IUserRepository userRepository,
		IVaccineRepository vaccineRepository,
		IPetImageSubmissionService imageSubmissionService,
		IValueProvider valueProvider)
	{
		_petRepository = petRepository ?? throw new ArgumentNullException(nameof(petRepository));
		_breedRepository = breedRepository ?? throw new ArgumentNullException(nameof(breedRepository));
		_speciesRepository = speciesRepository ?? throw new ArgumentNullException(nameof(speciesRepository));
		_colorRepository = colorRepository ?? throw new ArgumentNullException(nameof(colorRepository));
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_vaccineRepository = vaccineRepository ?? throw new ArgumentNullException(nameof(vaccineRepository));
		_imageSubmissionService =
			imageSubmissionService ?? throw new ArgumentNullException(nameof(imageSubmissionService));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
	}

	public async Task<PetResponse> GetPetBydIdAsync(Guid petId)
	{
		Pet searchedPet = await ValidateAndAssignPetAsync(petId);

		return searchedPet.ToPetResponse();
	}

	public async Task<PetResponse> CreatePetAsync(CreatePetRequest createPetRequest, Guid userId)
	{
		Breed breed = await ValidateAndAssignBreedAsync(createPetRequest.BreedId);
		Species species = await ValidateAndAssignSpeciesAsync(createPetRequest.SpeciesId);
		List<Color> colors = await ValidateAndAssignColorsAsync(createPetRequest.ColorIds);
		List<Vaccine> vaccines =
			await ValidateAndAssignVaccinesAsync(createPetRequest.VaccineIds, createPetRequest.SpeciesId);
		User petOwner = await ValidateAndAssignUserAsync(userId);

		Guid petId = _valueProvider.NewGuid();

		if (createPetRequest.Images.Count >= 10)
		{
			throw new BadRequestException("Não é possível adicionar 10 ou mais imagens");
		}

		Pet petToBeCreated = new()
		{
			Id = petId,
			Name = createPetRequest.Name,
			Observations = createPetRequest.Observations,
			Gender = createPetRequest.Gender,
			AgeInMonths = createPetRequest.AgeInMonths,
			Owner = petOwner,
			Breed = breed,
			Species = species,
			Colors = colors,
			Vaccines = vaccines,
			Images = new List<PetImage>(0),
		};

		List<PetImage> uploadedImages = await UploadPetImages(petToBeCreated, createPetRequest.Images);

		bool success = await _petRepository.CreatePetAndImages(petToBeCreated, uploadedImages);
		if (!success)
		{
			throw new InternalServerErrorException();
		}

		return petToBeCreated.ToPetResponse();
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

		var uploadedImageUrls =
			await _imageSubmissionService.UpdatePetImageAsync(dbPet.Id, editPetRequest.Images, dbPet.Images.Count);
		List<PetImage> uploadedImages = uploadedImageUrls
			.Select(image => new PetImage() { ImageUrl = image, Pet = dbPet, PetId = dbPet.Id })
			.ToList();

		dbPet.Id = editPetRequest.Id;
		dbPet.Name = editPetRequest.Name;
		dbPet.Observations = editPetRequest.Observations;
		dbPet.Gender = editPetRequest.Gender;
		dbPet.Images = uploadedImages;
		dbPet.AgeInMonths = editPetRequest.AgeInMonths;
		dbPet.Owner = petOwner;
		dbPet.Breed = breed;
		dbPet.Colors = colors;
		dbPet.Species = species;

		await _petRepository.CommitAsync();

		return dbPet.ToPetResponse();
	}

	public async Task DeletePetAsync(Guid petId, Guid userId)
	{
		Pet petToDelete = await ValidateAndAssignPetAsync(petId);
		if (petToDelete.Owner.Id != userId)
		{
			throw new UnauthorizedException("Você não possui permissão para excluir o animal.");
		}

		await _imageSubmissionService.DeletePetImageAsync(petToDelete.Id, petToDelete.Images);

		_petRepository.Delete(petToDelete);
		await _petRepository.CommitAsync();
	}

	public async Task<PetResponse> UpdateVaccinations(PetVaccinationRequest petVaccinationRequest, Guid petId,
		Guid userId)
	{
		Pet? vaccinatedPet = await _petRepository.GetPetByIdAsync(petId);
		if (vaccinatedPet is null)
		{
			throw new NotFoundException("Animal com o id especificado não existe.");
		}

		if (vaccinatedPet.Owner.Id != userId)
		{
			throw new ForbiddenException("Você não possui permissão para adicionar vacinas ao animal.");
		}

		var appliedVaccines = await _vaccineRepository.GetMultipleByIdAsync(petVaccinationRequest.VaccinationIds);
		if (appliedVaccines.Count != petVaccinationRequest.VaccinationIds.Count)
		{
			throw new NotFoundException("Alguma vacina com o id especificado não existe.");
		}

		ValidateIfVaccinesAreFromCorrectSpecies(appliedVaccines, vaccinatedPet.Species.Id);

		vaccinatedPet.Vaccines = appliedVaccines;
		await _petRepository.CommitAsync();

		return vaccinatedPet.ToPetResponse();
	}

	private static void ValidateIfVaccinesAreFromCorrectSpecies(List<Vaccine> vaccines, int speciesId)
	{
		bool isValid = vaccines.All(vaccine => vaccine.Species.Any(species => species.Id == speciesId));
		if (!isValid)
		{
			throw new BadRequestException("Não é possível adicionar vacinas de outras espécies.");
		}
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

	private async Task<List<Vaccine>> ValidateAndAssignVaccinesAsync(List<int> vaccineIds, int speciesId)
	{
		List<Vaccine> vaccines = await _vaccineRepository.GetMultipleByIdAsync(vaccineIds);
		if (vaccines.Count != vaccineIds.Count || vaccines.Count == 0)
		{
			throw new NotFoundException("Alguma das vacinas especificadas não existe.");
		}

		ValidateIfVaccinesAreFromCorrectSpecies(vaccines, speciesId);

		return vaccines;
	}

	private async Task<List<PetImage>> UploadPetImages(Pet petToBeCreated, List<IFormFile> submittedImages)
	{
		var uploadedImageUrls = await _imageSubmissionService.UploadPetImageAsync(petToBeCreated.Id, submittedImages);

		return uploadedImageUrls
			.Select(image => new PetImage() { ImageUrl = image, PetId = petToBeCreated.Id, Pet = petToBeCreated })
			.ToList();
	}
}