using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Domain.Entities.Alerts.UserPreferences;

namespace Application.Common.Extensions.Mapping.Alerts.UserPreferences;

public static class FoundAnimalUserPreferencesMappings
{
	public static FoundAnimalUserPreferencesResponse ToFoundAnimalUserPreferencesResponse(
		this FoundAnimalUserPreferences userPreferencesMappings)
	{
		return new FoundAnimalUserPreferencesResponse()
		{
			Id = userPreferencesMappings.Id,
			User = userPreferencesMappings.User.ToUserDataResponse(),
			Colors = userPreferencesMappings.Colors.ToListOfColorResponse(),
			Species = userPreferencesMappings.Species?.ToSpeciesResponse(),
			Breed = userPreferencesMappings.Breed?.ToBreedResponse(),
			Gender = userPreferencesMappings.Gender?.ToString(),
			FoundLocationLatitude = userPreferencesMappings.FoundLocationLatitude,
			FoundLocationLongitude = userPreferencesMappings.FoundLocationLongitude,
			RadiusDistanceInKm = userPreferencesMappings.RadiusDistanceInKm
		};
	}
}