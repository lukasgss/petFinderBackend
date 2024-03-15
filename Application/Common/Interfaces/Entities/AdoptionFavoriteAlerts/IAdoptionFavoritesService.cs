using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;

namespace Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts;

public interface IAdoptionFavoritesService
{
	Task<AdoptionFavoriteResponse> GetByIdAsync(Guid favoriteId, Guid userId);
	Task<PaginatedEntity<AdoptionFavoriteResponse>> GetAllAdoptionFavorites(Guid userId, int pageNumber, int pageSize);
	Task<AdoptionFavoriteResponse> ToggleFavorite(Guid userId, Guid alertId);
}