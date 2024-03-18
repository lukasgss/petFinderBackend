using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Domain.Entities.Alerts.UserPreferences;
using Domain.Enums;

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
			Age = userPreferences.Age is null ? null : Enum.GetName(typeof(Age), userPreferences.Age),
			Breed = userPreferences.Breed?.ToBreedResponse(),
			Gender = userPreferences.Gender is null ? null : Enum.GetName(typeof(Gender), userPreferences.Gender),
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
			Age = userPreferences.Age is null ? null : Enum.GetName(typeof(Age), userPreferences.Age),
			Species = userPreferences.Species?.ToSpeciesResponse(),
			Breed = userPreferences.Breed?.ToBreedResponse(),
			Gender = userPreferences.Gender is null ? null : Enum.GetName(typeof(Gender), userPreferences.Gender),
			FoundLocationLatitude = userPreferences.Location?.Y,
			FoundLocationLongitude = userPreferences.Location?.X,
			RadiusDistanceInKm = userPreferences.RadiusDistanceInKm
		};
	}
}