using Domain.Enums;

namespace Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;

public class MissingAlertFilters : BaseAlertFilters
{
	public bool Missing { get; init; } = true;
	public bool NotMissing { get; init; }
	public int? BreedId { get; init; }
	public Gender? GenderId { get; init; }
	public int? SpeciesId { get; init; }
	public int? ColorId { get; init; }
}