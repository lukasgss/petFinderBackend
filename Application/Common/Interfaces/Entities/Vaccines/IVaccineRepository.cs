using Application.Common.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Common.Interfaces.Entities.Vaccines;

public interface IVaccineRepository : IGenericRepository<Vaccine>
{
	Task<List<Vaccine>> GetMultipleByIdAsync(List<int> vaccineId);
}