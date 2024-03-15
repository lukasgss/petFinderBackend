using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts.DTOs;
using Domain.Entities.Alerts.UserFavorites;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class AdoptionFavoriteMappings
{
	public static AdoptionFavoriteResponse ToAdoptionFavoriteResponse(this AdoptionFavorite adoptionFavorite)
	{
		return new AdoptionFavoriteResponse()
		{
			Id = adoptionFavorite.Id,
			AdoptionAlert = adoptionFavorite.AdoptionAlert.ToSimplifiedAdoptionAlertResponse()
		};
	}
}