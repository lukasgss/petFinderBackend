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
			Species = userPreferences.Species.ToListOfSpeciesResponse(),
			Ages = userPreferences.Ages.ToListOfAgeResponse(),
			Breeds = userPreferences.Breeds.ToListOfBreedResponse(),
			Genders = userPreferences.Genders.ToListOfGenderResponse(),
			Sizes = userPreferences.Sizes.ToListOfSizeResponse(),
			FoundLocationLatitude = userPreferences.Location?.Y,
			FoundLocationLongitude = userPreferences.Location?.X,
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
			Ages = userPreferences.Ages.ToListOfAgeResponse(),
			Species = userPreferences.Species.ToListOfSpeciesResponse(),
			Breeds = userPreferences.Breeds.ToListOfBreedResponse(),
			Genders = userPreferences.Genders.ToListOfGenderResponse(),
			Sizes = userPreferences.Sizes.ToListOfSizeResponse(),
			FoundLocationLatitude = userPreferences.Location?.Y,
			FoundLocationLongitude = userPreferences.Location?.X,
			RadiusDistanceInKm = userPreferences.RadiusDistanceInKm
		};
	}
}