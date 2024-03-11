using Application.Common.Interfaces.GenericRepository;
using Domain.Entities.Alerts.UserPreferences;

namespace Application.Common.Interfaces.Entities.Alerts.UserPreferences.AdoptionAlerts;

public interface IAdoptionUserPreferencesRepository : IGenericRepository<AdoptionUserPreferences>
{
	Task<AdoptionUserPreferences?> GetUserPreferences(Guid userId);
}