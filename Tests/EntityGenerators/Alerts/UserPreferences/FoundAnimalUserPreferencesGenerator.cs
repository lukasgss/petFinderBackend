using Application.Common.Calculators;
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
			Age = Constants.FoundAnimalUserPreferencesData.Age,
			Location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
				Constants.FoundAnimalUserPreferencesData.FoundLocationLatitude!.Value,
				Constants.FoundAnimalUserPreferencesData.FoundLocationLongitude!.Value),
			RadiusDistanceInKm = Constants.FoundAnimalUserPreferencesData.RadiusDistanceInKm,
			Species = Constants.FoundAnimalUserPreferencesData.Species,
			SpeciesId = Constants.FoundAnimalUserPreferencesData.SpeciesId,
			Breed = Constants.FoundAnimalUserPreferencesData.Breed,
			BreedId = Constants.FoundAnimalUserPreferencesData.BreedId,
			Gender = Constants.FoundAnimalUserPreferencesData.Gender,
		};
	}
}