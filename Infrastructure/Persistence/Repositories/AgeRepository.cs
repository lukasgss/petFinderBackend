using Application.Common.Interfaces.Entities.Ages;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AgeRepository : IAgeRepository
{
	private readonly AppDbContext _dbContext;

	public AgeRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<Age?> GetByIdAsync(int ageId)
	{
		return await _dbContext.Ages
			.SingleOrDefaultAsync(age => age.Id == ageId);
	}

	public async Task<List<Age>> GetAll()
	{
		return await _dbContext.Ages
			.AsNoTracking()
			.ToListAsync();
	}
}