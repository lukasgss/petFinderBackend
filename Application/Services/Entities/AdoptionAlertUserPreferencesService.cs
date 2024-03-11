using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Application.Common.Interfaces.General.UserPreferences;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts.UserPreferences;

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
		List<Color> colors =
			await _userPreferencesValidations.ValidateAndAssignColorsAsync(createUserPreferences.ColorIds);
		User user = await _userPreferencesValidations.AssignUserAsync(userId);

		AdoptionUserPreferences adoptionUserPreferences = new()
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
		List<Color> colors =
			await _userPreferencesValidations.ValidateAndAssignColorsAsync(editUserPreferences.ColorIds);
		User user = await _userPreferencesValidations.AssignUserAsync(userId);

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

		await _adoptionUserPreferencesRepository.CommitAsync();

		return dbUserPreferences.ToAdoptionUserPreferencesResponse();
	}
}