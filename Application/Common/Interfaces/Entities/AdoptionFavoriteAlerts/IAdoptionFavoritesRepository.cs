using Application.Common.Interfaces.GenericRepository;
using Application.Common.Pagination;
using Domain.Entities.Alerts.UserFavorites;

namespace Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts;

public interface IAdoptionFavoritesRepository : IGenericRepository<AdoptionFavorite>
{
	Task<AdoptionFavorite?> GetByIdAsync(Guid favoriteId, Guid userId);
	Task<PagedList<AdoptionFavorite>> ListFavoritesAsync(Guid userId, int pageNumber, int pageSize);
	Task<AdoptionFavorite?> GetFavoriteAlertAsync(Guid userId, Guid alertId);
}