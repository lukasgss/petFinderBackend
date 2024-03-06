using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Domain.Entities.Alerts;
using Domain.ValueObjects;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class FoundAnimalAlertMappings
{
	public static FoundAnimalAlertResponse ToFoundAnimalAlertResponse(this FoundAnimalAlert foundAnimalAlert)
	{
		return new FoundAnimalAlertResponse()
		{
			Id = foundAnimalAlert.Id,
			Name = foundAnimalAlert.Name,
			Description = foundAnimalAlert.Description,
			FoundLocationLatitude = foundAnimalAlert.FoundLocationLatitude,
			FoundLocationLongitude = foundAnimalAlert.FoundLocationLongitude,
			RegistrationDate = foundAnimalAlert.RegistrationDate,
			RecoveryDate = foundAnimalAlert.RecoveryDate,
			Images = foundAnimalAlert.Images.ToFoundAlertImagesResponse(),
			Species = foundAnimalAlert.Species.ToSpeciesResponse(),
			Breed = foundAnimalAlert.Breed?.ToBreedResponse(),
			Owner = foundAnimalAlert.User.ToUserDataResponse(),
			Gender = foundAnimalAlert.Gender.ToString(),
			Colors = foundAnimalAlert.Colors.ToListOfColorResponse()
		};
	}

	private static List<string> ToFoundAlertImagesResponse(this IEnumerable<FoundAnimalAlertImage> images)
	{
		return images.Select(alertImage => alertImage.ImageUrl).ToList();
	}
}