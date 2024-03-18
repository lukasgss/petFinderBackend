using Domain.Enums;

namespace Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;

public class CreateAlertsUserPreferences
{
	public double? FoundLocationLatitude { get; set; }
	public double? FoundLocationLongitude { get; set; }
	public double? RadiusDistanceInKm { get; set; }
	public Gender? Gender { get; set; }
	public int? AgeId { get; set; }
	public int? SpeciesId { get; set; }
	public int? BreedId { get; set; }
	public required List<int> ColorIds { get; set; }
}