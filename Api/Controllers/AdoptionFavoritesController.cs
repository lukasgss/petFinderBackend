using Application.ApplicationConstants;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts;
using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/favorite/adoptions")]
public class AdoptionFavoritesController : ControllerBase
{
	private readonly IAdoptionFavoritesService _adoptionFavoritesService;
	private readonly IUserAuthorizationService _userAuthorizationService;

	public AdoptionFavoritesController(IAdoptionFavoritesService adoptionFavoritesService,
		IUserAuthorizationService userAuthorizationService)
	{
		_adoptionFavoritesService = adoptionFavoritesService ??
		                            throw new ArgumentNullException(nameof(adoptionFavoritesService));
		_userAuthorizationService = userAuthorizationService ??
		                            throw new ArgumentNullException(nameof(userAuthorizationService));
	}

	[Authorize]
	[HttpGet("{favoriteId:guid}", Name = "GetFavoriteByIdAsync")]
	public async Task<ActionResult<AdoptionFavoriteResponse>> GetFavoriteByIdAsync(Guid favoriteId)
	{
		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		return await _adoptionFavoritesService.GetByIdAsync(favoriteId, userId);
	}

	[Authorize]
	[HttpGet]
	public async Task<ActionResult<PaginatedEntity<AdoptionFavoriteResponse>>> GetAllFavorites(
		int page = 1, int pageSize = Constants.DefaultPageSize)
	{
		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		return await _adoptionFavoritesService.GetAllAdoptionFavorites(userId, page, pageSize);
	}

	[Authorize]
	[HttpPost("{alertId:guid}")]
	public async Task<ActionResult<AdoptionFavoriteResponse>> ToggleFavorite(Guid alertId)
	{
		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		AdoptionFavoriteResponse adoptionFavorite = await _adoptionFavoritesService.ToggleFavorite(userId, alertId);
		return new CreatedAtRouteResult(
			nameof(GetFavoriteByIdAsync),
			new { favoriteId = adoptionFavorite.Id },
			adoptionFavorite);
	}
}