using Application.Common.Interfaces.Entities.Alerts.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts;

public interface IMissingAlertService
{
    Task<MissingAlertResponse> GetMissingAlertByIdAsync(Guid missingAlertId);

    Task<MissingAlertResponse> CreateMissingAlertAsync(
        CreateMissingAlertRequest createMissingAlertRequest,
        Guid? userId);
}