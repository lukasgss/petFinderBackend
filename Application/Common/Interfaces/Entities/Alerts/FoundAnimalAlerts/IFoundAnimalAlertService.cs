using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;

public interface IFoundAnimalAlertService
{
	Task<FoundAnimalAlertResponse> GetByIdAsync(Guid alertId);
	Task<FoundAnimalAlertResponse> CreateAsync(CreateFoundAnimalAlertRequest createAlertRequest, Guid userId);
	Task<FoundAnimalAlertResponse> EditAsync(EditFoundAnimalAlertRequest editAlertRequest, Guid userId, Guid routeId);
	Task DeleteAsync(Guid alertId, Guid userId);
}