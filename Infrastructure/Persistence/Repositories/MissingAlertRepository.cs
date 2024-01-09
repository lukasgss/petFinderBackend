using System.Device.Location;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;
using Application.Common.Pagination;
using Domain.Entities.Alerts;
using Infrastructure.Persistence.DataContext;
using Infrastructure.Persistence.QueryLogics;
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

    public async Task<PagedList<MissingAlert>> ListMissingAlertsWithGeoFilters(
        MissingAlertFilters filters, int pageNumber, int pageSize)
    {
        var query = _dbContext.MissingAlerts
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Colors)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Breed)
            .Include(alert => alert.User)
            .Where(CoordinatesCalculator.MissingAlertIsWithinRadiusDistance(
                new GeoCoordinate(filters.Latitude, filters.Longitude),
                filters.RadiusDistanceInKm))
            // filters records based if it should show only adopted alerts
            // (AdoptionDate != null), show only non adopted alerts
            // (AdoptionDate == null) or both, if both filters.NotAdopted
            // or filters.Adopted are true
            .Where(alert => alert.RecoveryDate == null == filters.NotMissing ||
                            alert.RecoveryDate != null == filters.Missing);

        return await PagedList<MissingAlert>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public async Task<PagedList<MissingAlert>> ListMissingAlertsWithStatusFilters(
        MissingAlertFilters filters, int pageNumber, int pageSize)
    {
        var query = _dbContext.MissingAlerts
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Colors)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Breed)
            .Include(alert => alert.User)
            // filters records based if it should show only adopted alerts
            // (AdoptionDate != null), show only non adopted alerts
            // (AdoptionDate == null) or both, if both filters.NotAdopted
            // or filters.Adopted are true
            .Where(alert => alert.RecoveryDate == null == filters.Missing ||
                            alert.RecoveryDate != null == filters.NotMissing);

        return await PagedList<MissingAlert>.ToPagedListAsync(query, pageNumber, pageSize);
    }
}