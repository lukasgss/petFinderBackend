using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Pagination;
using Domain.Entities.Alerts;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using System.Device.Location;
using Infrastructure.Persistence.QueryLogics;

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
            .ThenInclude(pet => pet.Colors)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Breed)
            .Include(alert => alert.User)
            .SingleOrDefaultAsync(alert => alert.Id == alertId);
    }

    public async Task<PagedList<AdoptionAlert>> ListAdoptionAlerts(int pageNumber, int pageSize)
    {
        var query = _dbContext.AdoptionAlerts
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Colors)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Breed)
            .Include(alert => alert.User);

        return await PagedList<AdoptionAlert>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public async Task<PagedList<AdoptionAlert>> ListAdoptionAlertsWithFilters(
        AdoptionAlertFilters filters, int pageNumber, int pageSize)
    {
        var query = _dbContext.AdoptionAlerts
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Colors)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Breed)
            .Include(alert => alert.User)
            .Where(CoordinatesCalculator.IsWithinRadiusDistance(
                new GeoCoordinate(filters.Latitude, filters.Longitude),
                filters.RadiusDistanceInKm));

        return await PagedList<AdoptionAlert>.ToPagedListAsync(query, pageNumber, pageSize);
    }
}