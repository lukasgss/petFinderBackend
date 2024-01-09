namespace Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;

public class MissingAlertFilters : BaseAlertFilters
{
    public bool Missing { get; set; } = false;
    public bool NotMissing { get; set; } = true;
}