using Application.Common.Interfaces.Entities.Alerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;
using Application.Common.Pagination;
using Domain.Entities.Alerts;
using GeoCoordinatePortable;
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
			.ThenInclude(pet => pet.Images)
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Breed)
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Colors)
			.Include(alert => alert.User)
			.SingleOrDefaultAsync(alert => alert.Id == id);
	}

	public async Task<PagedList<MissingAlert>> ListMissingAlerts(
		MissingAlertFilters filters, int pageNumber, int pageSize)
	{
		var query = _dbContext.MissingAlerts
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Images)
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Colors)
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Breed)
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Vaccines)
			.Include(alert => alert.User)
			// filters records based if it should show only missing alerts
			// (RecoveryDate != null), show only non recovered alerts
			// (RecoveryDate == null) or both, if both filters.Missing
			// or filters.NotMissing are true
			.Where(alert => alert.RecoveryDate == null == filters.Missing ||
			                alert.RecoveryDate != null == filters.NotMissing);

		query = ApplyFilters(query, filters);

		return await PagedList<MissingAlert>.ToPagedListAsync(query, pageNumber, pageSize);
	}

	private static IQueryable<MissingAlert> ApplyFilters(IQueryable<MissingAlert> query, MissingAlertFilters filters)
	{
		if (AlertFilters.HasGeoFilters(filters))
		{
			query = query.Where(CoordinatesCalculator.MissingAlertIsWithinRadiusDistance(
				new GeoCoordinate((double)filters.Latitude!, (double)filters.Longitude!),
				(double)filters.RadiusDistanceInKm!));
		}

		if (filters.BreedId is not null)
		{
			query = query.Where(alert => alert.Pet.Breed.Id == filters.BreedId);
		}

		if (filters.GenderId is not null)
		{
			query = query.Where(alert => alert.Pet.Gender == filters.GenderId);
		}

		if (filters.SpeciesId is not null)
		{
			query = query.Where(alert => alert.Pet.Species.Id == filters.SpeciesId);
		}

		if (filters.ColorId is not null)
		{
			query = query.Where(alert => alert.Pet.Colors.Any(color => color.Id == filters.ColorId));
		}

		return query;
	}
}