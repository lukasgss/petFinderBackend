using Application.Common.Interfaces.GenericRepository;
using Domain.Entities.Alerts.UserPreferences;

namespace Application.Common.Interfaces.Entities.Alerts.UserPreferences.FoundAnimalAlerts;

public interface IFoundAnimalUserPreferencesRepository : IGenericRepository<FoundAnimalUserPreferences>
{
	Task<FoundAnimalUserPreferences?> GetUserPreferences(Guid userId);
}