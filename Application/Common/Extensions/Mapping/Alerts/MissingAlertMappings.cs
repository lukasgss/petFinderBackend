using Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;
using Domain.Entities.Alerts;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class MissingAlertMappings
{
    public static MissingAlertResponse ToMissingAlertResponse(this MissingAlert missingAlert)
    {
        return new MissingAlertResponse()
        {
            Id = missingAlert.Id,
            RegistrationDate = missingAlert.RegistrationDate,
            LastSeenLocationLatitude = missingAlert.LastSeenLocationLatitude,
            LastSeenLocationLongitude = missingAlert.LastSeenLocationLongitude,
            RecoveryDate = missingAlert.RecoveryDate,
            Pet = missingAlert.Pet.ToPetResponseNoOwner(
                missingAlert.Pet.Colors,
                missingAlert.Pet.Breed),
            Owner = missingAlert.User.ToOwnerResponse()
        };
    }
}