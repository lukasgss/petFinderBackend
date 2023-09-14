using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Domain.Entities.Alerts;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AdoptionAlertRepository : GenericRepository<AdoptionAlert>, IAdoptionAlertRepository
{
    private readonly AppDbContext _dbContext;
    
    public AdoptionAlertRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<AdoptionAlert?> GetByIdAsync(Guid alertId)
    {
        return await _dbContext.AdoptionAlerts
            .Include(alert => alert.Pet)
            .Include(alert => alert.User)
            .SingleOrDefaultAsync(alert => alert.Id == alertId);
    }
}