using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.DTOs;

public class MissingAlertResponse
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; } = null!;
    public string OwnerPhoneNumber { get; set; } = null!;
    public DateTime RegistrationDate { get; set; }
    public double LastSeenLocationLatitude { get; set; }
    public double LastSeenLocationLongitude { get; set; }
    public DateOnly? RecoveryDate { get; set; }
    public PetResponseNoOwner Pet { get; set; } = null!;
    public OwnerResponse Owner { get; set; } = null!;
}