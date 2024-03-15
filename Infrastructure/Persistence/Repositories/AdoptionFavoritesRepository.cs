using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts;
using Application.Common.Pagination;
using Domain.Entities.Alerts.UserFavorites;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AdoptionFavoritesRepository : GenericRepository<AdoptionFavorite>, IAdoptionFavoritesRepository
{
	private readonly AppDbContext _dbContext;

	public AdoptionFavoritesRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<AdoptionFavorite?> GetByIdAsync(Guid favoriteId, Guid userId)
	{
		return await _dbContext.AdoptionFavorites
			.Include(favorite => favorite.User)
			.Include(favorite => favorite.AdoptionAlert)
			.Include(favorite => favorite.AdoptionAlert.Pet)
			.ThenInclude(pet => pet.Images)
			.SingleOrDefaultAsync(favorite => favorite.Id == favoriteId && favorite.UserId == userId);
	}

	public async Task<PagedList<AdoptionFavorite>> ListFavoritesAsync(Guid userId, int pageNumber, int pageSize)
	{
		var query = _dbContext.AdoptionFavorites
			.Include(favorite => favorite.User)
			.Include(favorite => favorite.AdoptionAlert)
			.Include(favorite => favorite.AdoptionAlert.Pet)
			.ThenInclude(pet => pet.Images)
			.Where(favorite => favorite.User.Id == userId);

		return await PagedList<AdoptionFavorite>.ToPagedListAsync(query, pageNumber, pageSize);
	}

	public async Task<AdoptionFavorite?> GetFavoriteAlertAsync(Guid userId, Guid alertId)
	{
		return await _dbContext.AdoptionFavorites
			.SingleOrDefaultAsync(favorite => favorite.UserId == userId && favorite.AdoptionAlertId == alertId);
	}
}