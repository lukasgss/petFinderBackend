using Application.Common.Calculators;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.FoundAnimalAlerts;
using Application.Common.Interfaces.General.UserPreferences;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts.UserPreferences;
using NetTopologySuite.Geometries;

namespace Application.Services.Entities;

public class FoundAnimalUserPreferencesService : IFoundAnimalUserPreferencesService
{
	private readonly IFoundAnimalUserPreferencesRepository _foundAnimalUserPreferencesRepository;
	private readonly IUserPreferencesValidations _userPreferencesValidations;
	private readonly IValueProvider _valueProvider;


	public FoundAnimalUserPreferencesService(
		IFoundAnimalUserPreferencesRepository foundAnimalUserPreferencesRepository,
		IUserPreferencesValidations userPreferencesValidations,
		IValueProvider valueProvider)
	{
		_foundAnimalUserPreferencesRepository = foundAnimalUserPreferencesRepository ??
		                                        throw new ArgumentNullException(
			                                        nameof(foundAnimalUserPreferencesRepository));
		_userPreferencesValidations = userPreferencesValidations ??
		                              throw new ArgumentNullException(nameof(userPreferencesValidations));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
	}

	public async Task<UserPreferencesResponse> GetUserPreferences(Guid currentUserId)
	{
		FoundAnimalUserPreferences? userPreferences =
			await _foundAnimalUserPreferencesRepository.GetUserPreferences(currentUserId);
		if (userPreferences is null)
		{
			throw new NotFoundException(
				"Ainda não foram definidas preferências desse tipo de alerta para este usuário.");
		}

		return userPreferences.ToFoundAnimalUserPreferencesResponse();
	}

	public async Task<UserPreferencesResponse> CreatePreferences(
		CreateAlertsUserPreferences createUserPreferences, Guid userId)
	{
		FoundAnimalUserPreferences? dbUserPreferences =
			await _foundAnimalUserPreferencesRepository.GetUserPreferences(userId);
		if (dbUserPreferences is not null)
		{
			throw new BadRequestException("Usuário já possui preferências cadastradas para esse tipo de alerta.");
		}

		Species? species =
			await _userPreferencesValidations.ValidateAndAssignSpeciesAsync(createUserPreferences.SpeciesId);
		Age? age = await _userPreferencesValidations.ValidateAndAssignAgeAsync(createUserPreferences.AgeId);
		Breed? breed =
			await _userPreferencesValidations.ValidateAndAssignBreedAsync(createUserPreferences.BreedId, species?.Id);
		List<Color> colors =
			await _userPreferencesValidations.ValidateAndAssignColorsAsync(createUserPreferences.ColorIds);
		User user = await _userPreferencesValidations.AssignUserAsync(userId);

		Point? location = null;
		if (createUserPreferences.FoundLocationLatitude is not null &&
		    createUserPreferences.FoundLocationLongitude is not null &&
		    createUserPreferences.RadiusDistanceInKm is not null)
		{
			location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
				createUserPreferences.FoundLocationLatitude.Value,
				createUserPreferences.FoundLocationLongitude!.Value);
		}

		FoundAnimalUserPreferences foundAnimalUserPreferences = new()
		{
			Id = _valueProvider.NewGuid(),
			User = user,
			UserId = user.Id,
			Colors = colors,
			Breed = breed,
			Species = species,
			Age = age,
			Gender = createUserPreferences.Gender,
			Location = location,
			RadiusDistanceInKm = createUserPreferences.RadiusDistanceInKm
		};

		_foundAnimalUserPreferencesRepository.Add(foundAnimalUserPreferences);

		await _foundAnimalUserPreferencesRepository.CommitAsync();
		return foundAnimalUserPreferences.ToFoundAnimalUserPreferencesResponse();
	}

	public async Task<UserPreferencesResponse> EditPreferences(
		EditAlertsUserPreferences editUserPreferences, Guid userId)
	{
		FoundAnimalUserPreferences? dbUserPreferences =
			await _foundAnimalUserPreferencesRepository.GetUserPreferences(userId);
		if (dbUserPreferences is null)
		{
			throw new BadRequestException("Usuário não possui preferências cadastradas para esse tipo de alerta.");
		}

		Species? species =
			await _userPreferencesValidations.ValidateAndAssignSpeciesAsync(editUserPreferences.SpeciesId);
		Age? age = await _userPreferencesValidations.ValidateAndAssignAgeAsync(editUserPreferences.AgeId);
		Breed? breed =
			await _userPreferencesValidations.ValidateAndAssignBreedAsync(editUserPreferences.BreedId, species?.Id);
		List<Color> colors =
			await _userPreferencesValidations.ValidateAndAssignColorsAsync(editUserPreferences.ColorIds);
		User user = await _userPreferencesValidations.AssignUserAsync(userId);

		Point? location = null;
		if (editUserPreferences.FoundLocationLatitude is not null &&
		    editUserPreferences.FoundLocationLongitude is not null &&
		    editUserPreferences.RadiusDistanceInKm is not null)
		{
			location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
				editUserPreferences.FoundLocationLatitude.Value,
				editUserPreferences.FoundLocationLongitude!.Value);
		}

		dbUserPreferences.User = user;
		dbUserPreferences.UserId = user.Id;
		dbUserPreferences.Colors = colors;
		dbUserPreferences.Breed = breed;
		dbUserPreferences.Species = species;
		dbUserPreferences.Age = age;
		dbUserPreferences.Gender = editUserPreferences.Gender;
		dbUserPreferences.Location = location;
		dbUserPreferences.RadiusDistanceInKm = editUserPreferences.RadiusDistanceInKm;

		await _foundAnimalUserPreferencesRepository.CommitAsync();

		return dbUserPreferences.ToFoundAnimalUserPreferencesResponse();
	}
}