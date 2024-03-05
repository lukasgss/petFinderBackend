﻿using Application.Common.Interfaces.Entities.Alerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Pagination;
using Domain.Entities.Alerts;
using GeoCoordinatePortable;
using Infrastructure.Persistence.DataContext;
using Infrastructure.Persistence.QueryLogics;
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

	public async Task<PagedList<FoundAnimalAlert>> ListMissingAlerts(
		FoundAnimalAlertFilters filters, int pageNumber, int pageSize)
	{
		var query = _dbContext.FoundAnimalAlerts
			.Include(alert => alert.Species)
			.Include(alert => alert.Breed)
			.Include(alert => alert.Colors)
			.Include(alert => alert.User)
			.AsNoTracking()
			.AsQueryable();

		query = ApplyFilters(query, filters);

		return await PagedList<FoundAnimalAlert>.ToPagedListAsync(query, pageNumber, pageSize);
	}

	private static IQueryable<FoundAnimalAlert> ApplyFilters(IQueryable<FoundAnimalAlert> query,
		FoundAnimalAlertFilters filters)
	{
		if (AlertFilters.HasGeoFilters(filters))
		{
			query = query.Where(CoordinatesCalculator.FoundAnimalAlertIsWithinRadiusDistance(
				new GeoCoordinate((double)filters.Latitude!, (double)filters.Longitude!),
				(double)filters.RadiusDistanceInKm!));
		}

		if (filters.Name is not null)
		{
			string nameWithDatabaseWildcards = $"%{filters.Name}%";

			query = query.Where(alert =>
				alert.Name != null &&
				EF.Functions.ILike(EF.Functions.Unaccent(alert.Name), nameWithDatabaseWildcards));
		}

		if (filters.BreedId is not null)
		{
			query = query.Where(alert => alert.Breed != null && alert.Breed.Id == filters.BreedId);
		}

		if (filters.GenderId is not null)
		{
			query = query.Where(alert => alert.Gender == filters.GenderId);
		}

		if (filters.SpeciesId is not null)
		{
			query = query.Where(alert => alert.Species.Id == filters.SpeciesId);
		}

		if (filters.ColorId is not null)
		{
			query = query.Where(alert => alert.Colors.Any(color => color.Id == filters.ColorId));
		}

		return query;
	}
}