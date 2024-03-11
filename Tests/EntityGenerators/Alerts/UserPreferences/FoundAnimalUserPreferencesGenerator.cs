using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Domain.Entities.Alerts.UserPreferences;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators.Alerts.UserPreferences;

public static class FoundAnimalUserPreferencesGenerator
{
	public static FoundAnimalUserPreferences GenerateFoundAnimalUserPreferences()
	{
		return new FoundAnimalUserPreferences()
		{
			Id = Constants.FoundAnimalUserPreferencesData.Id,
			User = Constants.FoundAnimalUserPreferencesData.User,
			UserId = Constants.FoundAnimalUserPreferencesData.User.Id,
			Colors = Constants.FoundAnimalUserPreferencesData.Colors,
			FoundLocationLatitude = Constants.FoundAnimalUserPreferencesData.FoundLocationLatitude,
			FoundLocationLongitude = Constants.FoundAnimalUserPreferencesData.FoundLocationLongitude,
			RadiusDistanceInKm = Constants.FoundAnimalUserPreferencesData.RadiusDistanceInKm,
			Species = Constants.FoundAnimalUserPreferencesData.Species,
			SpeciesId = Constants.FoundAnimalUserPreferencesData.SpeciesId,
			Breed = Constants.FoundAnimalUserPreferencesData.Breed,
			BreedId = Constants.FoundAnimalUserPreferencesData.BreedId,
			Gender = Constants.FoundAnimalUserPreferencesData.Gender,
		};
	}

	public static CreateFoundAnimalUserPreferences GenerateCreateFoundAnimalUserPreferences()
	{
		return new CreateFoundAnimalUserPreferences()
		{
			ColorIds = Constants.FoundAnimalUserPreferencesData.ColorIds,
			SpeciesId = Constants.FoundAnimalUserPreferencesData.SpeciesId,
			FoundLocationLatitude = Constants.FoundAnimalUserPreferencesData.FoundLocationLatitude,
			FoundLocationLongitude = Constants.FoundAnimalUserPreferencesData.FoundLocationLongitude,
			RadiusDistanceInKm = Constants.FoundAnimalUserPreferencesData.RadiusDistanceInKm,
			Gender = Constants.FoundAnimalUserPreferencesData.Gender,
			BreedId = Constants.FoundAnimalUserPreferencesData.BreedId
		};
	}
}