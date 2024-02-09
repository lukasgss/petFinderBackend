using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Domain.Entities.Alerts;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class FoundAnimalAlertRepository : GenericRepository<FoundAnimalAlert>, IFoundAnimalAlertRepository
{
	private readonly AppDbContext _dbContext;

	public FoundAnimalAlertRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<FoundAnimalAlert?> GetByIdAsync(Guid alertId)
	{
		return await _dbContext.FoundAnimalAlerts
			.Include(alert => alert.Breed)
			.Include(alert => alert.Species)
			.Include(alert => alert.Colors)
			.Include(alert => alert.User)
			.FirstOrDefaultAsync(alert => alert.Id == alertId);
	}
}