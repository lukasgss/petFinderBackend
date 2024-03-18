using Application.Common.Calculators;
using Domain.Entities.Alerts.UserPreferences;
using Constants = Tests.TestUtils.Constants.Constants;


namespace Tests.EntityGenerators.Alerts.UserPreferences;

public static class AdoptionUserPreferencesGenerator
{
	public static AdoptionUserPreferences GenerateAdoptionUserPreferences()
	{
		return new AdoptionUserPreferences()
		{
			Id = Constants.AdoptionAlertUserPreferencesData.Id,
			User = Constants.AdoptionAlertUserPreferencesData.User,
			UserId = Constants.AdoptionAlertUserPreferencesData.User.Id,
			Colors = Constants.AdoptionAlertUserPreferencesData.Colors,
			Age = AgeGenerator.GenerateAge(),
			Location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
				Constants.AdoptionAlertUserPreferencesData.Latitude!.Value,
				Constants.AdoptionAlertUserPreferencesData.Longitude!.Value),
			RadiusDistanceInKm = Constants.AdoptionAlertUserPreferencesData.RadiusDistanceInKm,
			Species = Constants.AdoptionAlertUserPreferencesData.Species,
			SpeciesId = Constants.AdoptionAlertUserPreferencesData.SpeciesId,
			Breed = Constants.AdoptionAlertUserPreferencesData.Breed,
			BreedId = Constants.AdoptionAlertUserPreferencesData.BreedId,
			Gender = Constants.AdoptionAlertUserPreferencesData.Gender,
		};
	}
}