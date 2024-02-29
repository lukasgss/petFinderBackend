using Application.Common.Interfaces.Entities.Vaccines;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VaccineRepository : GenericRepository<Vaccine>, IVaccineRepository
{
	private readonly AppDbContext _dbContext;

	public VaccineRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<List<Vaccine>> GetMultipleByIdAsync(List<int> vaccineIds)
	{
		return await _dbContext.Vaccines
			.Include(vaccine => vaccine.Species)
			.Where(vaccine => vaccineIds.Contains(vaccine.Id))
			.ToListAsync();
	}

	public async Task<List<Vaccine>> GetVaccinesOfSpecies(int speciesId)
	{
		return await _dbContext.Vaccines
			.Include(vaccine => vaccine.Species)
			.Where(vaccine => vaccine.Species.Any(species => species.Id == speciesId))
			.AsNoTracking()
			.ToListAsync();
	}
}