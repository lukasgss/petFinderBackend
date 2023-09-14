using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Domain.Entities.Alerts;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class AdoptionAlertMappings
{
    public static AdoptionAlertResponse ToAdoptionAlertResponse(this AdoptionAlert adoptionAlert)
    {
        return new AdoptionAlertResponse()
        {
            Id = adoptionAlert.Id,
            OnlyForScreenedProperties = adoptionAlert.OnlyForScreenedProperties,
            LocationLatitude = adoptionAlert.LocationLatitude,
            LocationLongitude = adoptionAlert.LocationLongitude,
            Description = adoptionAlert.Description,
            RegistrationDate = adoptionAlert.RegistrationDate,
            AdoptionDate = adoptionAlert.AdoptionDate,
            Pet = adoptionAlert.Pet.ToPetResponseNoOwner(adoptionAlert.Pet.Colors, adoptionAlert.Pet.Breed),
            Owner = adoptionAlert.User.ToUserDataResponse()
        };
    }
}