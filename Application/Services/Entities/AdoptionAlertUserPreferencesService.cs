using Application.Common.Calculators;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Application.Common.Interfaces.General.UserPreferences;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts.UserPreferences;
using NetTopologySuite.Geometries;

namespace Application.Services.Entities;

public class AdoptionAlertUserPreferencesService : IAdoptionAlertUserPreferencesService
{
	private readonly IAdoptionUserPreferencesRepository _adoptionUserPreferencesRepository;
	private readonly IUserPreferencesValidations _userPreferencesValidations;
	private readonly IValueProvider _valueProvider;

	public AdoptionAlertUserPreferencesService(
		IAdoptionUserPreferencesRepository adoptionUserPreferencesRepository,
		IUserPreferencesValidations userPreferencesValidations,
		IValueProvider valueProvider)
	{
		_adoptionUserPreferencesRepository = adoptionUserPreferencesRepository ??
		                                     throw new ArgumentNullException(nameof(adoptionUserPreferencesRepository));
		_userPreferencesValidations = userPreferencesValidations ??
		                              throw new ArgumentNullException(nameof(userPreferencesValidations));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
	}

	public async Task<UserPreferencesResponse> GetUserPreferences(Guid currentUserId)
	{
		AdoptionUserPreferences? userPreferences =
			await _adoptionUserPreferencesRepository.GetUserPreferences(currentUserId);
		if (userPreferences is null)
		{
			throw new NotFoundException(
				"Ainda não foram definidas preferências desse tipo de alerta para este usuário.");
		}

		return userPreferences.ToAdoptionUserPreferencesResponse();
	}

	public async Task<UserPreferencesResponse> CreatePreferences(CreateAlertsUserPreferences createUserPreferences,
		Guid userId)
	{
		AdoptionUserPreferences? dbUserPreferences =
			await _adoptionUserPreferencesRepository.GetUserPreferences(userId);
		if (dbUserPreferences is not null)
		{
			throw new BadRequestException("Usuário já possui preferências cadastradas para esse tipo de alerta.");
		}

		Species? species =
			await _userPreferencesValidations.ValidateAndAssignSpeciesAsync(createUserPreferences.SpeciesId);
		Breed? breed =
			await _userPreferencesValidations.ValidateAndAssignBreedAsync(createUserPreferences.BreedId, species?.Id);
		Age? age = await _userPreferencesValidations.ValidateAndAssignAgeAsync(createUserPreferences.AgeId);
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

		AdoptionUserPreferences adoptionUserPreferences = new()
		{
			Id = _valueProvider.NewGuid(),
			User = user,
			Colors = colors,
			Age = age,
			Breed = breed,
			Species = species,
			Gender = createUserPreferences.Gender,
			Location = location,
			RadiusDistanceInKm = createUserPreferences.RadiusDistanceInKm
		};

		_adoptionUserPreferencesRepository.Add(adoptionUserPreferences);
		await _adoptionUserPreferencesRepository.CommitAsync();

		return adoptionUserPreferences.ToAdoptionUserPreferencesResponse();
	}

	public async Task<UserPreferencesResponse> EditPreferences(
		EditAlertsUserPreferences editUserPreferences, Guid userId)
	{
		AdoptionUserPreferences? dbUserPreferences =
			await _adoptionUserPreferencesRepository.GetUserPreferences(userId);
		if (dbUserPreferences is null)
		{
			throw new BadRequestException("Usuário não possui preferências cadastradas para esse tipo de alerta.");
		}

		Species? species =
			await _userPreferencesValidations.ValidateAndAssignSpeciesAsync(editUserPreferences.SpeciesId);
		Breed? breed =
			await _userPreferencesValidations.ValidateAndAssignBreedAsync(editUserPreferences.BreedId, species?.Id);
		Age? age = await _userPreferencesValidations.ValidateAndAssignAgeAsync(editUserPreferences.AgeId);
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
		dbUserPreferences.Colors = colors;
		dbUserPreferences.Breed = breed;
		dbUserPreferences.Species = species;
		dbUserPreferences.Gender = editUserPreferences.Gender;
		dbUserPreferences.Age = age;
		dbUserPreferences.Location = location;
		dbUserPreferences.RadiusDistanceInKm = editUserPreferences.RadiusDistanceInKm;

		await _adoptionUserPreferencesRepository.CommitAsync();

		return dbUserPreferences.ToAdoptionUserPreferencesResponse();
	}
}