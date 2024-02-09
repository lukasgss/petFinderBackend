using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Domain.Entities.Alerts;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators.Alerts;

public static class FoundAnimalAlertGenerator
{
	public static FoundAnimalAlert GenerateFoundAnimalAlert()
	{
		return new FoundAnimalAlert()
		{
			Id = Constants.FoundAnimalAlertData.Id,
			Name = Constants.FoundAnimalAlertData.Name,
			Description = Constants.FoundAnimalAlertData.Description,
			FoundLocationLatitude = Constants.FoundAnimalAlertData.FoundLocationLatitude,
			FoundLocationLongitude = Constants.FoundAnimalAlertData.FoundLocationLongitude,
			RegistrationDate = Constants.FoundAnimalAlertData.RegistrationDate,
			HasBeenRecovered = Constants.FoundAnimalAlertData.HasBeenRecovered,
			Image = Constants.FoundAnimalAlertData.Image,
			Species = Constants.FoundAnimalAlertData.Species,
			SpeciesId = Constants.FoundAnimalAlertData.SpeciesId,
			Breed = Constants.FoundAnimalAlertData.Breed,
			BreedId = Constants.FoundAnimalAlertData.BreedId,
			User = Constants.FoundAnimalAlertData.User,
			UserId = Constants.FoundAnimalAlertData.UserId,
			Gender = Constants.FoundAnimalAlertData.Gender,
			Colors = Constants.FoundAnimalAlertData.Colors
		};
	}

	public static CreateFoundAnimalAlertRequest GenerateCreateFoundAnimalAlertRequest()
	{
		return new CreateFoundAnimalAlertRequest()
		{
			Name = Constants.FoundAnimalAlertData.Name,
			Description = Constants.FoundAnimalAlertData.Description,
			FoundLocationLatitude = Constants.FoundAnimalAlertData.FoundLocationLatitude,
			FoundLocationLongitude = Constants.FoundAnimalAlertData.FoundLocationLongitude,
			Image = Constants.FoundAnimalAlertData.ImageFile,
			SpeciesId = Constants.FoundAnimalAlertData.SpeciesId,
			Gender = Constants.FoundAnimalAlertData.Gender,
			BreedId = Constants.FoundAnimalAlertData.BreedId,
			ColorIds = Constants.FoundAnimalAlertData.ColorIds
		};
	}

	public static FoundAnimalAlertResponse GenerateFoundAnimalAlertResponse()
	{
		return new FoundAnimalAlertResponse()
		{
			Id = Constants.FoundAnimalAlertData.Id,
			Name = Constants.FoundAnimalAlertData.Name,
			Description = Constants.FoundAnimalAlertData.Description,
			FoundLocationLatitude = Constants.FoundAnimalAlertData.FoundLocationLatitude,
			FoundLocationLongitude = Constants.FoundAnimalAlertData.FoundLocationLongitude,
			RegistrationDate = Constants.FoundAnimalAlertData.RegistrationDate,
			HasBeenRecovered = Constants.FoundAnimalAlertData.HasBeenRecovered,
			Image = Constants.FoundAnimalAlertData.Image,
			Species = Constants.FoundAnimalAlertData.Species.ToSpeciesResponse(),
			Breed = Constants.FoundAnimalAlertData.Breed.ToBreedResponse(),
			Owner = Constants.FoundAnimalAlertData.User.ToUserDataResponse(),
			Gender = Constants.FoundAnimalAlertData.Gender.ToString(),
			Colors = Constants.FoundAnimalAlertData.Colors.ToListOfColorResponse()
		};
	}
}