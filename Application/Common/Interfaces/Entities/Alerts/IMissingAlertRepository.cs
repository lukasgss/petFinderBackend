using Application.Common.Interfaces.GenericRepository;
using Domain.Entities.Alerts;

namespace Application.Common.Interfaces.Entities.Alerts;

public interface IMissingAlertRepository : IGenericRepository<MissingAlert>
{
    Task<MissingAlert?> GetByIdAsync(Guid id);
}