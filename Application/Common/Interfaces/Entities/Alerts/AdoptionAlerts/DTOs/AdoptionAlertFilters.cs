namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

public class AdoptionAlertFilters : BaseAlertFilters
{
    public bool Adopted { get; set; } = false;
    public bool NotAdopted { get; set; } = true;
}