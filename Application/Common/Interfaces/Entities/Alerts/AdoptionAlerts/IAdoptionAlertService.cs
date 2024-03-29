using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;

namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;

public interface IAdoptionAlertService
{
    Task<AdoptionAlertResponse> GetByIdAsync(Guid alertId);

    Task<PaginatedEntity<AdoptionAlertResponse>> ListAdoptionAlerts(
        AdoptionAlertFilters filters,
        int page,
        int pageSize);

    Task<AdoptionAlertResponse> CreateAsync(CreateAdoptionAlertRequest createAlertRequest, Guid userId);
    Task<AdoptionAlertResponse> EditAsync(EditAdoptionAlertRequest editAlertRequest, Guid userId, Guid routeId);
    Task DeleteAsync(Guid alertId, Guid userId);
    Task<AdoptionAlertResponse> ToggleAdoptionAsync(Guid alertId, Guid userId);
}