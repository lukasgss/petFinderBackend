using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts.UserPreferences;

namespace Application.Services.Entities;

public class FoundAnimalUserPreferencesService : IFoundAnimalUserPreferencesService
{
	private readonly IFoundAnimalUserPreferencesRepository _foundAnimalUserPreferencesRepository;
	private readonly IBreedRepository _breedRepository;
	private readonly ISpeciesRepository _speciesRepository;
	private readonly IColorRepository _colorRepository;
	private readonly IUserRepository _userRepository;
	private readonly IValueProvider _valueProvider;


	public FoundAnimalUserPreferencesService(
		IFoundAnimalUserPreferencesRepository foundAnimalUserPreferencesRepository,
		IBreedRepository breedRepository,
		ISpeciesRepository speciesRepository,
		IColorRepository colorRepository,
		IUserRepository userRepository,
		IValueProvider valueProvider)
	{
		_foundAnimalUserPreferencesRepository = foundAnimalUserPreferencesRepository ??
		                                        throw new ArgumentNullException(
			                                        nameof(foundAnimalUserPreferencesRepository));
		_breedRepository = breedRepository ?? throw new ArgumentNullException(nameof(breedRepository));
		_speciesRepository = speciesRepository ?? throw new ArgumentNullException(nameof(speciesRepository));
		_colorRepository = colorRepository ?? throw new ArgumentNullException(nameof(colorRepository));
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
	}

	public async Task<FoundAnimalUserPreferencesResponse> GetUserPreferences(Guid currentUserId)
	{
		FoundAnimalUserPreferences? userPreferences =
			await _foundAnimalUserPreferencesRepository.GetUserPreferences(currentUserId);
		if (userPreferences is null)
		{
			throw new NotFoundException("Ainda não foram definidas preferências para este usuário.");
		}

		return userPreferences.ToFoundAnimalUserPreferencesResponse();
	}

	public async Task<FoundAnimalUserPreferencesResponse> CreatePreferences(
		CreateFoundAnimalUserPreferences createUserPreferences, Guid userId)
	{
		FoundAnimalUserPreferences? dbUserPreferences =
			await _foundAnimalUserPreferencesRepository.GetUserPreferences(userId);
		if (dbUserPreferences is not null)
		{
			throw new BadRequestException("Usuário já possui preferências cadastradas.");
		}

		Species? species = await ValidateAndAssignSpeciesAsync(createUserPreferences.SpeciesId);
		Breed? breed = await ValidateAndAssignBreedAsync(createUserPreferences.BreedId, species?.Id);
		List<Color> colors = await ValidateAndAssignColorsAsync(createUserPreferences.ColorIds);
		User user = await AssignUserAsync(userId);

		FoundAnimalUserPreferences foundAnimalUserPreferences = new()
		{
			Id = _valueProvider.NewGuid(),
			User = user,
			UserId = user.Id,
			Colors = colors,
			Breed = breed,
			BreedId = breed?.Id,
			Species = species,
			SpeciesId = species?.Id,
			Gender = createUserPreferences.Gender,
			FoundLocationLatitude = createUserPreferences.FoundLocationLatitude,
			FoundLocationLongitude = createUserPreferences.FoundLocationLongitude,
			RadiusDistanceInKm = createUserPreferences.RadiusDistanceInKm
		};

		_foundAnimalUserPreferencesRepository.Add(foundAnimalUserPreferences);
		await _foundAnimalUserPreferencesRepository.CommitAsync();

		return foundAnimalUserPreferences.ToFoundAnimalUserPreferencesResponse();
	}

	public async Task<FoundAnimalUserPreferencesResponse> EditPreferences(
		EditFoundAnimalUserPreferences editUserPreferences, Guid userId)
	{
		FoundAnimalUserPreferences? dbUserPreferences =
			await _foundAnimalUserPreferencesRepository.GetUserPreferences(userId);
		if (dbUserPreferences is null)
		{
			throw new BadRequestException("Usuário não possui preferências cadastradas.");
		}

		Species? species = await ValidateAndAssignSpeciesAsync(editUserPreferences.SpeciesId);
		Breed? breed = await ValidateAndAssignBreedAsync(editUserPreferences.BreedId, species?.Id);
		List<Color> colors = await ValidateAndAssignColorsAsync(editUserPreferences.ColorIds);
		User user = await AssignUserAsync(userId);

		dbUserPreferences.User = user;
		dbUserPreferences.UserId = user.Id;
		dbUserPreferences.Colors = colors;
		dbUserPreferences.Breed = breed;
		dbUserPreferences.BreedId = breed?.Id;
		dbUserPreferences.Species = species;
		dbUserPreferences.SpeciesId = species?.Id;
		dbUserPreferences.Gender = editUserPreferences.Gender;
		dbUserPreferences.FoundLocationLatitude = editUserPreferences.FoundLocationLatitude;
		dbUserPreferences.FoundLocationLongitude = editUserPreferences.FoundLocationLongitude;
		dbUserPreferences.RadiusDistanceInKm = editUserPreferences.RadiusDistanceInKm;

		await _foundAnimalUserPreferencesRepository.CommitAsync();

		return dbUserPreferences.ToFoundAnimalUserPreferencesResponse();
	}

	private async Task<User> AssignUserAsync(Guid userId)
	{
		return (await _userRepository.GetUserByIdAsync(userId))!;
	}

	private async Task<Breed?> ValidateAndAssignBreedAsync(int? breedId, int? speciesId)
	{
		if (breedId is null)
		{
			return null;
		}

		Breed? breed = await _breedRepository.GetBreedByIdAsync((int)breedId);
		if (breed is null)
		{
			throw new NotFoundException("Raça especificada não existe.");
		}

		if (breed.Species.Id != speciesId && speciesId is not null)
		{
			throw new BadRequestException("Raça não pertence a espécie especificada.");
		}

		return breed;
	}

	private async Task<Species?> ValidateAndAssignSpeciesAsync(int? speciesId)
	{
		if (speciesId is null)
		{
			return null;
		}

		Species? species = await _speciesRepository.GetSpeciesByIdAsync((int)speciesId);
		if (species is null)
		{
			throw new NotFoundException("Espécie especificada não existe.");
		}

		return species;
	}

	private async Task<List<Color>> ValidateAndAssignColorsAsync(List<int> colorIds)
	{
		if (!colorIds.Any())
		{
			return new List<Color>(0);
		}

		List<Color> colors = await _colorRepository.GetMultipleColorsByIdsAsync(colorIds);
		if (colors.Count != colorIds.Count || colors.Count == 0)
		{
			throw new NotFoundException("Alguma das cores especificadas não existe.");
		}

		return colors;
	}
}