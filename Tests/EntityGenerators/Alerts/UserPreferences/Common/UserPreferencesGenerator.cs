using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators.Alerts.UserPreferences.Common;

public static class UserPreferencesGenerator
{
	public static CreateAlertsUserPreferences GenerateCreateFoundAnimalUserPreferences()
	{
		return new CreateAlertsUserPreferences()
		{
			BreedIds = Constants.FoundAnimalUserPreferencesData.BreedIds,
			SpeciesIds = Constants.FoundAnimalUserPreferencesData.SpeciesIds,
			ColorIds = Constants.FoundAnimalUserPreferencesData.ColorIds,
			FoundLocationLatitude = Constants.FoundAnimalUserPreferencesData.FoundLocationLatitude,
			FoundLocationLongitude = Constants.FoundAnimalUserPreferencesData.FoundLocationLongitude,
			Sizes = Constants.FoundAnimalUserPreferencesData.Sizes,
			Ages = Constants.FoundAnimalUserPreferencesData.Ages,
			RadiusDistanceInKm = Constants.FoundAnimalUserPreferencesData.RadiusDistanceInKm,
			Genders = Constants.FoundAnimalUserPreferencesData.Genders,
		};
	}
}