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
			Ages = Constants.AdoptionAlertUserPreferencesData.Ages,
			Location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
				Constants.AdoptionAlertUserPreferencesData.Latitude!.Value,
				Constants.AdoptionAlertUserPreferencesData.Longitude!.Value),
			RadiusDistanceInKm = Constants.AdoptionAlertUserPreferencesData.RadiusDistanceInKm,
			Species = Constants.AdoptionAlertUserPreferencesData.Species,
			Breeds = Constants.AdoptionAlertUserPreferencesData.Breed,
			Genders = Constants.AdoptionAlertUserPreferencesData.Gender,
		};
	}
}