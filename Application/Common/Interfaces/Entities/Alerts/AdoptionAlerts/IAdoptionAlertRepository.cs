using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.GenericRepository;
using Application.Common.Pagination;
using Domain.Entities.Alerts;

namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;

public interface IAdoptionAlertRepository : IGenericRepository<AdoptionAlert>
{
	Task<AdoptionAlert?> GetByIdAsync(Guid alertId);

	Task<PagedList<AdoptionAlert>> ListAdoptionAlerts(
		AdoptionAlertFilters filters, int pageNumber, int pageSize);
}