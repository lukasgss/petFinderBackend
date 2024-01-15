using Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;

namespace Application.Common.Interfaces.Entities.Alerts.MissingAlerts;

public interface IMissingAlertService
{
	Task<MissingAlertResponse> GetByIdAsync(Guid missingAlertId);

	Task<PaginatedEntity<MissingAlertResponse>> ListMissingAlerts(
		MissingAlertFilters filters, int page, int pageSize);

	Task<MissingAlertResponse> CreateAsync(
		CreateMissingAlertRequest createMissingAlertRequest,
		Guid userId);

	Task<MissingAlertResponse> EditAsync(EditMissingAlertRequest editMissingAlertRequest, Guid userId, Guid routeId);
	Task DeleteAsync(Guid missingAlertId, Guid userId);
	Task<MissingAlertResponse> ToggleFoundStatusAsync(Guid alertId, Guid userId);
}