using Application.Common.Interfaces.Entities.Pets.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

public class SimplifiedAdoptionAlertResponse
{
	public required Guid Id { get; init; }
	public required bool OnlyForScreenedProperties { get; init; }
	public required double LocationLatitude { get; init; }
	public required double LocationLongitude { get; init; }
	public string? Description { get; init; }
	public required DateTime RegistrationDate { get; init; }
	public DateOnly? AdoptionDate { get; init; }
	public required SimplifiedPetResponse Pet { get; init; }
}