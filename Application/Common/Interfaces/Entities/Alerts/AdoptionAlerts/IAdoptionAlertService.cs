using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;

public interface IAdoptionAlertService
{
    Task<AdoptionAlertResponse> GetByIdAsync(Guid alertId);
    Task<AdoptionAlertResponse> CreateAsync(CreateAdoptionAlertRequest createAlertRequest, Guid userId);
}