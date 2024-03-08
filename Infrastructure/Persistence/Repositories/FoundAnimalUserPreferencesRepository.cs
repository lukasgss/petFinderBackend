using Application.Common.Interfaces.Entities.Alerts.UserPreferences;
using Domain.Entities.Alerts.UserPreferences;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class FoundAnimalUserPreferencesRepository : GenericRepository<FoundAnimalUserPreferences>,
	IFoundAnimalUserPreferencesRepository
{
	private readonly AppDbContext _dbContext;

	public FoundAnimalUserPreferencesRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<FoundAnimalUserPreferences?> GetUserPreferences(Guid userId)
	{
		return await _dbContext.FoundAnimalUserPreferences
			.Include(preferences => preferences.Breed)
			.Include(preferences => preferences.User)
			.Include(preferences => preferences.Colors)
			.Include(preferences => preferences.Species)
			.SingleOrDefaultAsync(preferences => preferences.User.Id == userId);
	}
}