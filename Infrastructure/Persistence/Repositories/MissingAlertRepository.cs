using Application.Common.Interfaces.Entities.Alerts;
using Domain.Entities.Alerts;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MissingAlertRepository : GenericRepository<MissingAlert>, IMissingAlertRepository
{
    private readonly AppDbContext _dbContext;

    public MissingAlertRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MissingAlert?> GetByIdAsync(Guid id)
    {
        return await _dbContext.MissingAlerts
            .Include(alert => alert.Pet)
                .ThenInclude(pet => pet.Breed)
            .Include(alert => alert.Pet)
                .ThenInclude(pet => pet.Colors)
            .Include(alert => alert.User)
            .SingleOrDefaultAsync(alert => alert.Id == id);
    }
}