using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;

public interface IFoundAnimalAlertService
{
	Task<FoundAnimalAlertResponse> GetByIdAsync(Guid alertId);

	Task<PaginatedEntity<FoundAnimalAlertResponse>> ListFoundAnimalAlerts(
		FoundAnimalAlertFilters filters, int page, int pageSize);

	Task<FoundAnimalAlertResponse> CreateAsync(CreateFoundAnimalAlertRequest createAlertRequest, Guid userId);
	Task<FoundAnimalAlertResponse> EditAsync(EditFoundAnimalAlertRequest editAlertRequest, Guid userId, Guid routeId);
	Task DeleteAsync(Guid alertId, Guid userId);
	Task<FoundAnimalAlertResponse> ToggleAlertStatus(Guid alertId, Guid userId);
}