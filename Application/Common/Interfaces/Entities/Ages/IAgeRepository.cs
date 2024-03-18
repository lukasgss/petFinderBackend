using Domain.Entities;

namespace Application.Common.Interfaces.Entities.Ages;

public interface IAgeRepository
{
	Task<Age?> GetByIdAsync(int ageId);
	Task<List<Age>> GetAll();
}