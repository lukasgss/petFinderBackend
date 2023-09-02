using Application.Common.Interfaces.Entities.Alerts.DTOs;
using Domain.Entities.Alerts;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class MissingAlertMappings
{
    public static MissingAlertResponse ConvertToMissingAlertResponse(this MissingAlert missingAlert)
    {
        return new MissingAlertResponse()
        {
            Id = missingAlert.Id,
            OwnerName = missingAlert.OwnerName,
            OwnerPhoneNumber = missingAlert.OwnerPhoneNumber,
            RegistrationDate = missingAlert.RegistrationDate,
            LastSeenLocationLatitude = missingAlert.LastSeenLocationLatitude,
            LastSeenLocationLongitude = missingAlert.LastSeenLocationLongitude,
            PetHasBeenRecovered = missingAlert.PetHasBeenRecovered,
            Pet = missingAlert.Pet.ConvertToPetResponseNoOwner(
                missingAlert.Pet.Colors.ConvertToListOfColorResponse(),
                missingAlert.Pet.Breed.ConvertToBreedResponse()),
            Owner = missingAlert.User?.ConvertToOwnerResponse()
        };
    }
}