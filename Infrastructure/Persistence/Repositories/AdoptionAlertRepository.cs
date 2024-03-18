using Application.Common.Calculators;
using Application.Common.Converters;
using Application.Common.Interfaces.Entities.Alerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Pagination;
using Domain.Entities.Alerts;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

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
			.ThenInclude(pet => pet.Images)
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Colors)
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Breed)
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Vaccines)
			.Include(alert => alert.User)
			.SingleOrDefaultAsync(alert => alert.Id == alertId);
	}

	public async Task<PagedList<AdoptionAlert>> ListAdoptionAlerts(
		AdoptionAlertFilters filters, int pageNumber, int pageSize)
	{
		var query = _dbContext.AdoptionAlerts
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Images)
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Colors)
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Breed)
			.Include(alert => alert.Pet)
			.ThenInclude(pet => pet.Vaccines)
			.Include(alert => alert.User)
			// filters records based if it should show only adopted alerts
			// (AdoptionDate != null), show only non adopted alerts
			// (AdoptionDate == null) or both, if both filters.NotAdopted
			// or filters.Adopted are true
			.Where(alert => alert.AdoptionDate == null == filters.NotAdopted ||
			                alert.AdoptionDate != null == filters.Adopted);

		query = ApplyFilters(query, filters);

		return await PagedList<AdoptionAlert>.ToPagedListAsync(query, pageNumber, pageSize);
	}

	private static IQueryable<AdoptionAlert> ApplyFilters(IQueryable<AdoptionAlert> query, AdoptionAlertFilters filters)
	{
		if (AlertFilters.HasGeoFilters(filters))
		{
			Point filtersLocation =
				CoordinatesCalculator.CreatePointBasedOnCoordinates(filters.Latitude!.Value, filters.Longitude!.Value);
			double filteredDistanceInMeters = UnitsConverter.ConvertKmToMeters(filters.RadiusDistanceInKm!.Value);

			query = query.Where(alert => alert.Location.Distance(filtersLocation) <= filteredDistanceInMeters);
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