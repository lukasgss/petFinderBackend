using System.Collections.Generic;
using Application.Common.Pagination;
using Domain.Entities;
using Domain.Entities.Alerts;
using Tests.EntityGenerators.Alerts;
using Tests.EntityGenerators.Alerts.Comments;

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
}