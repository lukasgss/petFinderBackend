using Application.Common.Converters;
using Application.Common.Interfaces.Entities.Alerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;
using Application.Common.Pagination;
using Domain.Entities.Alerts;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using CoordinatesCalculator = Application.Common.Calculators.CoordinatesCalculator;

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
			Point filterLocation =
				CoordinatesCalculator.CreatePointBasedOnCoordinates(filters.Latitude!.Value, filters.Longitude!.Value);
			double filteredDistanceInMeters = UnitsConverter.ConvertKmToMeters(filters.RadiusDistanceInKm!.Value);

			query = query.Where(alert => alert.Location.Distance(filterLocation) <= filteredDistanceInMeters);
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