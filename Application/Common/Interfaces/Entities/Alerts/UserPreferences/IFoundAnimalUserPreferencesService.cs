using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.UserPreferences;

public interface IFoundAnimalUserPreferencesService
{
	Task<FoundAnimalUserPreferencesResponse> GetUserPreferences(Guid currentUserId);

	Task<FoundAnimalUserPreferencesResponse> CreatePreferences(
		CreateFoundAnimalUserPreferences createUserPreferences, Guid userId);

	Task<FoundAnimalUserPreferencesResponse> EditPreferences(
		EditFoundAnimalUserPreferences editUserPreferences, Guid userId);
}