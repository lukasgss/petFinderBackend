using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts.DTOs;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Tests.EntityGenerators.Alerts;
using Tests.EntityGenerators.Alerts.Comments;
using Tests.EntityGenerators.Alerts.UserFavorites;

namespace Tests.EntityGenerators;

public static class PaginatedEntityGenerator
{
	public static PaginatedEntity<UserMessageResponse> GeneratePaginatedUserMessageResponse()
	{
		var userMessageResponses = UserMessageGenerator.GenerateListOfUserMessageResponses();
		return new PaginatedEntity<UserMessageResponse>()
		{
			Data = userMessageResponses,
			CurrentPage = 1,
			CurrentPageCount = userMessageResponses.Count,
			TotalPages = 1
		};
	}

	public static PaginatedEntity<AdoptionAlertResponse> GeneratePaginatedAdoptionAlertResponse()
	{
		var adoptionAlertResponses = AdoptionAlertGenerator.GenerateListOfAlertsResponse();
		return new PaginatedEntity<AdoptionAlertResponse>()
		{
			Data = adoptionAlertResponses,
			CurrentPage = 1,
			CurrentPageCount = adoptionAlertResponses.Count,
			TotalPages = 1
		};
	}

	public static PaginatedEntity<MissingAlertResponse> GeneratePaginatedMissingAlertResponse()
	{
		var missingAlertResponses = MissingAlertGenerator.GenerateListOfAlertsResponse();
		return new PaginatedEntity<MissingAlertResponse>()
		{
			Data = missingAlertResponses,
			CurrentPage = 1,
			CurrentPageCount = missingAlertResponses.Count,
			TotalPages = 1
		};
	}

	public static PaginatedEntity<FoundAnimalAlertResponse> GeneratePaginatedFoundAnimalAlertResponse()
	{
		var foundAnimalAlertResponses = FoundAnimalAlertGenerator.GenerateListOfAlertsResponse();
		return new PaginatedEntity<FoundAnimalAlertResponse>()
		{
			Data = foundAnimalAlertResponses,
			CurrentPage = 1,
			CurrentPageCount = foundAnimalAlertResponses.Count,
			TotalPages = 1
		};
	}

	public static PaginatedEntity<AlertCommentResponse> GeneratePaginatedMissingAlertCommentResponse()
	{
		var alertCommentResponse = GenericAlertCommentGenerator.GenerateListOfMissingAlertComment();
		return new PaginatedEntity<AlertCommentResponse>()
		{
			Data = alertCommentResponse,
			CurrentPage = 1,
			CurrentPageCount = alertCommentResponse.Count,
			TotalPages = 1
		};
	}

	public static PaginatedEntity<AlertCommentResponse> GeneratePaginatedAdoptionAlertCommentResponse()
	{
		var alertCommentResponse = GenericAlertCommentGenerator.GenerateListOfAdoptionAlertComment();
		return new PaginatedEntity<AlertCommentResponse>()
		{
			Data = alertCommentResponse,
			CurrentPage = 1,
			CurrentPageCount = alertCommentResponse.Count,
			TotalPages = 1
		};
	}

	public static PaginatedEntity<AdoptionFavoriteResponse> GeneratePaginatedAdoptionFavoriteResponse()
	{
		var adoptionFavoriteResponse = AdoptionFavoritesGenerator.GenerateListOfResponse();
		return new PaginatedEntity<AdoptionFavoriteResponse>()
		{
			Data = adoptionFavoriteResponse,
			CurrentPage = 1,
			CurrentPageCount = adoptionFavoriteResponse.Count,
			TotalPages = 1
		};
	}
}