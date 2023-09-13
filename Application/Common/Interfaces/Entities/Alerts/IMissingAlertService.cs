using Application.Common.Interfaces.Entities.Alerts.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts;

public interface IMissingAlertService
{
    Task<MissingAlertResponse> GetByIdAsync(Guid missingAlertId);

    Task<MissingAlertResponse> CreateAsync(
        CreateMissingAlertRequest createMissingAlertRequest,
        Guid userId);
    Task<MissingAlertResponse> EditAsync(EditMissingAlertRequest editMissingAlertRequest, Guid userId, Guid routeId);
    Task DeleteAsync(Guid missingAlertId, Guid userId);
    Task<MissingAlertResponse> MarkAsResolvedAsync(Guid alertId, Guid userId);
}