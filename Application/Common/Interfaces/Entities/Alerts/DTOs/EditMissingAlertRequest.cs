namespace Application.Common.Interfaces.Entities.Alerts.DTOs;

public class EditMissingAlertRequest
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; } = null!;
    public string OwnerPhoneNumber { get; set; } = null!;
    public double LastSeenLocationLatitude { get; set; }
    public double LastSeenLocationLongitude { get; set; }
    public Guid PetId { get; set; }
    public Guid UserId { get; set; }
}