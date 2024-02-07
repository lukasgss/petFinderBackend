namespace Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;

public class MissingAlertFilters : BaseAlertFilters
{
	public bool Missing { get; init; } = true;
	public bool NotMissing { get; init; }
}