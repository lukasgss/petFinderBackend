using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

namespace Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts.DTOs;

public class AdoptionFavoriteResponse
{
	public required Guid Id { get; init; }
	public required SimplifiedAdoptionAlertResponse AdoptionAlert { get; init; }
}