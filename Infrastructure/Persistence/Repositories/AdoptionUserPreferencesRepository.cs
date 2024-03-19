using Application.Common.Interfaces.Entities.Alerts.UserPreferences.AdoptionAlerts;
using Domain.Entities.Alerts.UserPreferences;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AdoptionUserPreferencesRepository : GenericRepository<AdoptionUserPreferences>,
	IAdoptionUserPreferencesRepository
{
	private readonly AppDbContext _dbContext;

	public AdoptionUserPreferencesRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<AdoptionUserPreferences?> GetUserPreferences(Guid userId)
	{
		return await _dbContext.AdoptionUserPreferences
			.Include(preferences => preferences.Breeds)
			.Include(preferences => preferences.User)
			.Include(preferences => preferences.Colors)
			.Include(preferences => preferences.Species)
			.SingleOrDefaultAsync(preferences => preferences.User.Id == userId);
	}
}