using System.Collections.Generic;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts.DTOs;
using Domain.Entities.Alerts.UserFavorites;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators.Alerts.UserFavorites;

public static class AdoptionFavoritesGenerator
{
	public static AdoptionFavorite GenerateAdoptionFavorite()
	{
		return new AdoptionFavorite()
		{
			Id = Constants.AdoptionFavoritesData.Id,
			User = Constants.AdoptionFavoritesData.User,
			UserId = Constants.AdoptionFavoritesData.User.Id,
			AdoptionAlert = Constants.AdoptionFavoritesData.AdoptionAlert,
			AdoptionAlertId = Constants.AdoptionFavoritesData.AdoptionAlert.Id,
		};
	}

	public static List<AdoptionFavorite> GenerateList()
	{
		List<AdoptionFavorite> adoptionFavorites = new();
		for (int i = 0; i < 3; i++)
		{
			adoptionFavorites.Add(GenerateAdoptionFavorite());
		}

		return adoptionFavorites;
	}

	public static List<AdoptionFavoriteResponse> GenerateListOfResponse()
	{
		List<AdoptionFavoriteResponse> adoptionFavoritesResponse = new();
		for (int i = 0; i < 3; i++)
		{
			adoptionFavoritesResponse.Add(GenerateAdoptionFavorite().ToAdoptionFavoriteResponse());
		}

		return adoptionFavoritesResponse;
	}

	public static AdoptionFavoriteResponse GenerateResponse()
	{
		return GenerateAdoptionFavorite().ToAdoptionFavoriteResponse();
	}
}