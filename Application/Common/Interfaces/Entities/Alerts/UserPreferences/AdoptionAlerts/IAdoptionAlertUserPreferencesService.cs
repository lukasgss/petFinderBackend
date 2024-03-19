using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.UserPreferences.AdoptionAlerts;

public interface IAdoptionAlertUserPreferencesService
{
	Task<UserPreferencesResponse> GetUserPreferences(Guid currentUserId);
	Task<UserPreferencesResponse> CreatePreferences(CreateAlertsUserPreferences createPreferences, Guid userId);
	Task<UserPreferencesResponse> EditPreferences(EditAlertsUserPreferences editPreferences, Guid userId);
}