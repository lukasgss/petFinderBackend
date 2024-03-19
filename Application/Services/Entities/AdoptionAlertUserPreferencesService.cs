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

	public async Task<UserPreferencesResponse> CreatePreferences(
		CreateAlertsUserPreferences createPreferences, Guid userId)
	{
		AdoptionUserPreferences? dbPreferences = await _adoptionUserPreferencesRepository.GetUserPreferences(userId);
		if (dbPreferences is not null)
		{
			throw new BadRequestException("Usuário já possui preferências cadastradas para esse tipo de alerta.");
		}

		var species = await _userPreferencesValidations.ValidateAndAssignSpeciesAsync(createPreferences.SpeciesIds);
		var breed = await _userPreferencesValidations.ValidateAndAssignBreedAsync(createPreferences.BreedIds, species);
		var colors = await _userPreferencesValidations.ValidateAndAssignColorsAsync(createPreferences.ColorIds);
		User user = await _userPreferencesValidations.AssignUserAsync(userId);

		Point? location = null;
		if (createPreferences.FoundLocationLatitude is not null &&
		    createPreferences.FoundLocationLongitude is not null &&
		    createPreferences.RadiusDistanceInKm is not null)
		{
			location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
				createPreferences.FoundLocationLatitude.Value,
				createPreferences.FoundLocationLongitude!.Value);
		}

		AdoptionUserPreferences adoptionUserPreferences = new()
		{
			Id = _valueProvider.NewGuid(),
			User = user,
			Colors = colors,
			Ages = createPreferences.Ages,
			Breeds = breed,
			Species = species,
			Sizes = createPreferences.Sizes,
			Genders = createPreferences.Genders,
			Location = location,
			RadiusDistanceInKm = createPreferences.RadiusDistanceInKm
		};

		_adoptionUserPreferencesRepository.Add(adoptionUserPreferences);
		await _adoptionUserPreferencesRepository.CommitAsync();

		return adoptionUserPreferences.ToAdoptionUserPreferencesResponse();
	}

	public async Task<UserPreferencesResponse> EditPreferences(
		EditAlertsUserPreferences editPreferences, Guid userId)
	{
		AdoptionUserPreferences? dbUserPreferences =
			await _adoptionUserPreferencesRepository.GetUserPreferences(userId);
		if (dbUserPreferences is null)
		{
			throw new BadRequestException("Usuário não possui preferências cadastradas para esse tipo de alerta.");
		}

		var species = await _userPreferencesValidations.ValidateAndAssignSpeciesAsync(editPreferences.SpeciesIds);
		var breeds = await _userPreferencesValidations.ValidateAndAssignBreedAsync(editPreferences.BreedIds, species);
		var colors = await _userPreferencesValidations.ValidateAndAssignColorsAsync(editPreferences.ColorIds);
		User user = await _userPreferencesValidations.AssignUserAsync(userId);

		Point? location = null;
		if (editPreferences.FoundLocationLatitude is not null &&
		    editPreferences.FoundLocationLongitude is not null &&
		    editPreferences.RadiusDistanceInKm is not null)
		{
			location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
				editPreferences.FoundLocationLatitude.Value,
				editPreferences.FoundLocationLongitude!.Value);
		}

		dbUserPreferences.User = user;
		dbUserPreferences.Colors = colors;
		dbUserPreferences.Breeds = breeds;
		dbUserPreferences.Species = species;
		dbUserPreferences.Sizes = editPreferences.Sizes;
		dbUserPreferences.Genders = editPreferences.Genders;
		dbUserPreferences.Ages = editPreferences.Ages;
		dbUserPreferences.Location = location;
		dbUserPreferences.RadiusDistanceInKm = editPreferences.RadiusDistanceInKm;

		await _adoptionUserPreferencesRepository.CommitAsync();

		return dbUserPreferences.ToAdoptionUserPreferencesResponse();
	}
}