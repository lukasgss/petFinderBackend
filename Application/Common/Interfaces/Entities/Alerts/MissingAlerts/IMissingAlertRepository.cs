using Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;
using Application.Common.Interfaces.GenericRepository;
using Application.Common.Pagination;
using Domain.Entities.Alerts;

namespace Application.Common.Interfaces.Entities.Alerts.MissingAlerts;

public interface IMissingAlertRepository : IGenericRepository<MissingAlert>
{
	Task<MissingAlert?> GetByIdAsync(Guid id);

	Task<PagedList<MissingAlert>> ListMissingAlerts(
		MissingAlertFilters filters, int pageNumber, int pageSize);
}