using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

public class AdoptionAlertResponse
{
    public Guid Id { get; set; }
    public bool OnlyForScreenedProperties { get; set; }
    public double LocationLatitude { get; set; }
    public double LocationLongitude { get; set; }
    public string? Description { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateOnly? AdoptionDate { get; set; }
    public PetResponseNoOwner Pet { get; set; } = null!;
    public UserDataResponse Owner { get; set; } = null!;
}