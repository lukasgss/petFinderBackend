namespace Application.Common.Interfaces.Entities.Alerts.DTOs;

public class EditMissingAlertRequest
{
    public Guid Id { get; set; }
    public double LastSeenLocationLatitude { get; set; }
    public double LastSeenLocationLongitude { get; set; }
    public Guid PetId { get; set; }
}