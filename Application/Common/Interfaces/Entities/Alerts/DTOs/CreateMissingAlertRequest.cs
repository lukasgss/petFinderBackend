namespace Application.Common.Interfaces.Entities.Alerts.DTOs;

public class CreateMissingAlertRequest
{
    public double LastSeenLocationLatitude { get; set; }
    public double LastSeenLocationLongitude { get; set; }
    public Guid PetId { get; set; }
}