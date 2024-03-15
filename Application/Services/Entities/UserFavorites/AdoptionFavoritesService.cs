using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts;
using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts.DTOs;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Entities.Alerts.UserFavorites;

namespace Application.Services.Entities.UserFavorites;

public class AdoptionFavoritesService : IAdoptionFavoritesService
{
	private readonly IAdoptionFavoritesRepository _adoptionFavoritesRepository;
	private readonly IAdoptionAlertRepository _adoptionAlertRepository;
	private readonly IUserRepository _userRepository;
	private readonly IValueProvider _valueProvider;

	public AdoptionFavoritesService(
		IAdoptionFavoritesRepository adoptionFavoritesRepository,
		IAdoptionAlertRepository adoptionAlertRepository,
		IUserRepository userRepository,
		IValueProvider valueProvider)
	{
		_adoptionFavoritesRepository = adoptionFavoritesRepository ??
		                               throw new ArgumentNullException(nameof(adoptionFavoritesRepository));
		_adoptionAlertRepository =
			adoptionAlertRepository ?? throw new ArgumentNullException(nameof(adoptionAlertRepository));
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
	}

	public async Task<AdoptionFavoriteResponse> GetByIdAsync(Guid favoriteId, Guid userId)
	{
		AdoptionFavorite? adoptionFavorite = await _adoptionFavoritesRepository.GetByIdAsync(favoriteId, userId);
		if (adoptionFavorite is null)
		{
			throw new NotFoundException("Não foi possível encontrar o item favoritado.");
		}

		return adoptionFavorite.ToAdoptionFavoriteResponse();
	}

	public async Task<PaginatedEntity<AdoptionFavoriteResponse>> GetAllAdoptionFavorites(
		Guid userId, int pageNumber, int pageSize)
	{
		var adoptionFavorites = await _adoptionFavoritesRepository.ListFavoritesAsync(userId, pageNumber, pageSize);

		return adoptionFavorites.ToAlertFavoritesResponse();
	}

	public async Task<AdoptionFavoriteResponse> ToggleFavorite(Guid userId, Guid alertId)
	{
		AdoptionAlert? adoptionAlert = await _adoptionAlertRepository.GetByIdAsync(alertId);
		if (adoptionAlert is null)
		{
			throw new NotFoundException("Alerta com o id especificado não existe.");
		}

		User user = (await _userRepository.GetUserByIdAsync(userId))!;

		AdoptionFavorite? adoptionFavorite = await _adoptionFavoritesRepository.GetFavoriteAlertAsync(userId, alertId);
		if (adoptionFavorite is null)
		{
			AdoptionFavorite favorite = new()
			{
				Id = _valueProvider.NewGuid(),
				User = user,
				UserId = user.Id,
				AdoptionAlert = adoptionAlert,
				AdoptionAlertId = adoptionAlert.Id
			};
			_adoptionFavoritesRepository.Add(favorite);
			await _adoptionFavoritesRepository.CommitAsync();

			return favorite.ToAdoptionFavoriteResponse();
		}

		_adoptionFavoritesRepository.Delete(adoptionFavorite);
		await _adoptionFavoritesRepository.CommitAsync();

		return adoptionFavorite.ToAdoptionFavoriteResponse();
	}
}