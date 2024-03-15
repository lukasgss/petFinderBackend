using System.Collections.Generic;
using Application.Common.Pagination;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Entities.Alerts.UserFavorites;
using Tests.EntityGenerators.Alerts;
using Tests.EntityGenerators.Alerts.Comments;
using Tests.EntityGenerators.Alerts.UserFavorites;

namespace Tests.EntityGenerators;

public static class PagedListGenerator
{
	public static PagedList<UserMessage> GeneratePagedUserMessages()
	{
		List<UserMessage> userMessages = UserMessageGenerator.GenerateListOfUserMessages();
		return new PagedList<UserMessage>(userMessages, userMessages.Count, pageNumber: 1, pageSize: 50);
	}

	public static PagedList<AdoptionAlert> GeneratePagedAdoptionAlerts()
	{
		List<AdoptionAlert> adoptionAlerts = AdoptionAlertGenerator.GenerateListOfAlerts();
		return new PagedList<AdoptionAlert>(adoptionAlerts, adoptionAlerts.Count, pageNumber: 1, pageSize: 50);
	}

	public static PagedList<MissingAlert> GeneratePagedMissingAlerts()
	{
		List<MissingAlert> missingAlerts = MissingAlertGenerator.GenerateListOfAlerts();
		return new PagedList<MissingAlert>(missingAlerts, missingAlerts.Count, pageNumber: 1, pageSize: 50);
	}

	public static PagedList<FoundAnimalAlert> GeneratePagedFoundAnimalAlerts()
	{
		List<FoundAnimalAlert> foundAnimalAlerts = FoundAnimalAlertGenerator.GenerateListOfAlerts();
		return new PagedList<FoundAnimalAlert>(foundAnimalAlerts, foundAnimalAlerts.Count, pageNumber: 1, pageSize: 50);
	}

	public static PagedList<MissingAlertComment> GeneratePagedMissingAlertsComments()
	{
		List<MissingAlertComment> alertComments = MissingAlertCommentGenerator.GenerateListOfMissingAlertComment();
		return new PagedList<MissingAlertComment>(alertComments, alertComments.Count, pageNumber: 1, pageSize: 50);
	}

	public static PagedList<AdoptionAlertComment> GeneratePagedAdoptionAlertComments()
	{
		List<AdoptionAlertComment> alertComments = AdoptionAlertCommentGenerator.GenerateListOfAdoptionAlertComments();
		return new PagedList<AdoptionAlertComment>(alertComments, alertComments.Count, pageNumber: 1, pageSize: 50);
	}

	public static PagedList<AdoptionFavorite> GeneratePagedAdoptionFavorites()
	{
		List<AdoptionFavorite> adoptionFavorites = AdoptionFavoritesGenerator.GenerateList();
		return new PagedList<AdoptionFavorite>(adoptionFavorites, adoptionFavorites.Count, pageNumber: 1, pageSize: 50);
	}
}