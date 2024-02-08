using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;

public interface IFoundAnimalAlertService
{
    Task<FoundAnimalAlertResponse> GetByIdAsync(Guid alertId);
    Task<FoundAnimalAlertResponse> CreateAsync(CreateFoundAnimalAlertRequest createAlertRequest, Guid userId);
}