using Application.Common.Interfaces.GenericRepository;
using Domain.Entities.Alerts;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;

public interface IFoundAnimalAlertRepository : IGenericRepository<FoundAnimalAlert>
{
    Task<FoundAnimalAlert?> GetByIdAsync(Guid alertId);
}