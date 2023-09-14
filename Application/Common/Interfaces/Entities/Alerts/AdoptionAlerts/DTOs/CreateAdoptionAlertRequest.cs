namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

public class CreateAdoptionAlertRequest
{
    public bool OnlyForScreenedProperties { get; set; }
    public double LocationLatitude { get; set; }
    public double LocationLongitude { get; set; }
    public string? Description { get; set; }
    public Guid PetId { get; set; }
}