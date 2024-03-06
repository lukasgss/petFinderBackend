using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.GenericRepository;
using Application.Common.Pagination;
using Domain.Entities.Alerts;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;

public interface IFoundAnimalAlertRepository : IGenericRepository<FoundAnimalAlert>
{
	Task<FoundAnimalAlert?> GetByIdAsync(Guid alertId);

	Task<PagedList<FoundAnimalAlert>> ListAlertsAsync(FoundAnimalAlertFilters filters, int pageNumber, int pageSize);
}