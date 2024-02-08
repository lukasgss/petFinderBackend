using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Domain.Entities.Alerts;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class FoundAnimalAlertMappings
{
    public static FoundAnimalAlertResponse ToFoundAnimalAlertResponse(this FoundAnimalAlert foundAnimalAlert)
    {
        return new FoundAnimalAlertResponse()
        {
            Id = foundAnimalAlert.Id,
            Name = foundAnimalAlert.Name,
            FoundLocationLatitude = foundAnimalAlert.FoundLocationLatitude,
            FoundLocationLongitude = foundAnimalAlert.FoundLocationLongitude,
            RegistrationDate = foundAnimalAlert.RegistrationDate,
            HasBeenRecovered = foundAnimalAlert.HasBeenRecovered,
            Image = foundAnimalAlert.Image,
            Species = foundAnimalAlert.Species.ToSpeciesResponse(),
            Breed = foundAnimalAlert.Breed?.ToBreedResponse(),
            Owner = foundAnimalAlert.User.ToUserDataResponse(),
            Gender = nameof(foundAnimalAlert.Gender),
            Colors = foundAnimalAlert.Colors.ToListOfColorResponse()
        };
    }
}