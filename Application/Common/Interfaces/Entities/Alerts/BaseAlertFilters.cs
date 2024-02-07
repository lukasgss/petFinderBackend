using Domain.Enums;

namespace Application.Common.Interfaces.Entities.Alerts;

public class BaseAlertFilters
{
	public double? Latitude { get; init; }
	public double? Longitude { get; init; }
	public double? RadiusDistanceInKm { get; init; }
	public int? BreedId { get; init; }
	public Gender? GenderId { get; init; }
	public int? SpeciesId { get; init; }
	public int? ColorId { get; init; }
}