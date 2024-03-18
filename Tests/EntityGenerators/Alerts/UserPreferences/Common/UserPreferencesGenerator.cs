using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators.Alerts.UserPreferences.Common;

public static class UserPreferencesGenerator
{
	public static CreateAlertsUserPreferences GenerateCreateFoundAnimalUserPreferences()
	{
		return new CreateAlertsUserPreferences()
		{
			ColorIds = Constants.FoundAnimalUserPreferencesData.ColorIds,
			SpeciesId = Constants.FoundAnimalUserPreferencesData.SpeciesId,
			FoundLocationLatitude = Constants.FoundAnimalUserPreferencesData.FoundLocationLatitude,
			FoundLocationLongitude = Constants.FoundAnimalUserPreferencesData.FoundLocationLongitude,
			AgeId = AgeGenerator.GenerateAge().Id,
			RadiusDistanceInKm = Constants.FoundAnimalUserPreferencesData.RadiusDistanceInKm,
			Gender = Constants.FoundAnimalUserPreferencesData.Gender,
			BreedId = Constants.FoundAnimalUserPreferencesData.BreedId
		};
	}
}