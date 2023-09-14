using Application.Common.Interfaces.GenericRepository;
using Domain.Entities.Alerts;

namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;

public interface IAdoptionAlertRepository : IGenericRepository<AdoptionAlert>
{
    Task<AdoptionAlert?> GetByIdAsync(Guid alertId);
}