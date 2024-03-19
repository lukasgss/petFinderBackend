using Domain.Enums;

namespace Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;

public class CreateAlertsUserPreferences
{
	public double? FoundLocationLatitude { get; init; }
	public double? FoundLocationLongitude { get; init; }
	public double? RadiusDistanceInKm { get; init; }
	public List<Gender>? Genders { get; init; }
	public List<Age>? Ages { get; init; }
	public List<Size>? Sizes { get; init; }
	public List<int>? SpeciesIds { get; init; }
	public List<int>? BreedIds { get; init; }
	public List<int>? ColorIds { get; init; }
}