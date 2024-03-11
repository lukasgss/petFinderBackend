using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Domain.Entities.Alerts.UserPreferences;

namespace Application.Common.Extensions.Mapping.Alerts.UserPreferences;

public static class UserPreferencesMappings
{
	public static UserPreferencesResponse ToFoundAnimalUserPreferencesResponse(
		this FoundAnimalUserPreferences userPreferences)
	{
		return new UserPreferencesResponse()
		{
			Id = userPreferences.Id,
			User = userPreferences.User.ToUserDataResponse(),
			Colors = userPreferences.Colors.ToListOfColorResponse(),
			Species = userPreferences.Species?.ToSpeciesResponse(),
			Breed = userPreferences.Breed?.ToBreedResponse(),
			Gender = userPreferences.Gender?.ToString(),
			FoundLocationLatitude = userPreferences.FoundLocationLatitude,
			FoundLocationLongitude = userPreferences.FoundLocationLongitude,
			RadiusDistanceInKm = userPreferences.RadiusDistanceInKm
		};
	}

	public static UserPreferencesResponse ToAdoptionUserPreferencesResponse(
		this AdoptionUserPreferences userPreferences)
	{
		return new UserPreferencesResponse()
		{
			Id = userPreferences.Id,
			User = userPreferences.User.ToUserDataResponse(),
			Colors = userPreferences.Colors.ToListOfColorResponse(),
			Species = userPreferences.Species?.ToSpeciesResponse(),
			Breed = userPreferences.Breed?.ToBreedResponse(),
			Gender = userPreferences.Gender?.ToString(),
			FoundLocationLatitude = userPreferences.FoundLocationLatitude,
			FoundLocationLongitude = userPreferences.FoundLocationLongitude,
			RadiusDistanceInKm = userPreferences.RadiusDistanceInKm
		};
	}
}