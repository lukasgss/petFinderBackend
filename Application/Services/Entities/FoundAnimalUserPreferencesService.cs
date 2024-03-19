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
		CreateAlertsUserPreferences createPreferences, Guid userId)
	{
		FoundAnimalUserPreferences? dbUserPreferences =
			await _foundAnimalUserPreferencesRepository.GetUserPreferences(userId);
		if (dbUserPreferences is not null)
		{
			throw new BadRequestException("Usuário já possui preferências cadastradas para esse tipo de alerta.");
		}

		var species = await _userPreferencesValidations.ValidateAndAssignSpeciesAsync(createPreferences.SpeciesIds);
		var breeds = await _userPreferencesValidations.ValidateAndAssignBreedAsync(createPreferences.BreedIds, species);
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

		FoundAnimalUserPreferences foundAnimalUserPreferences = new()
		{
			Id = _valueProvider.NewGuid(),
			User = user,
			UserId = user.Id,
			Colors = colors,
			Breeds = breeds,
			Species = species,
			Sizes = createPreferences.Sizes,
			Ages = createPreferences.Ages,
			Genders = createPreferences.Genders,
			Location = location,
			RadiusDistanceInKm = createPreferences.RadiusDistanceInKm
		};

		_foundAnimalUserPreferencesRepository.Add(foundAnimalUserPreferences);

		await _foundAnimalUserPreferencesRepository.CommitAsync();
		return foundAnimalUserPreferences.ToFoundAnimalUserPreferencesResponse();
	}

	public async Task<UserPreferencesResponse> EditPreferences(
		EditAlertsUserPreferences editPreferences, Guid userId)
	{
		FoundAnimalUserPreferences? dbPreferences =
			await _foundAnimalUserPreferencesRepository.GetUserPreferences(userId);
		if (dbPreferences is null)
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

		dbPreferences.User = user;
		dbPreferences.UserId = user.Id;
		dbPreferences.Colors = colors;
		dbPreferences.Breeds = breeds;
		dbPreferences.Species = species;
		dbPreferences.Sizes = editPreferences.Sizes;
		dbPreferences.Ages = editPreferences.Ages;
		dbPreferences.Genders = editPreferences.Genders;
		dbPreferences.Location = location;
		dbPreferences.RadiusDistanceInKm = editPreferences.RadiusDistanceInKm;

		await _foundAnimalUserPreferencesRepository.CommitAsync();

		return dbPreferences.ToFoundAnimalUserPreferencesResponse();
	}
}