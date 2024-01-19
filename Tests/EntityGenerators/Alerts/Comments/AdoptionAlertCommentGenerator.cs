using System.Collections.Generic;
using Domain.Entities;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators.Alerts.Comments;

public static class AdoptionAlertCommentGenerator
{
	public static AdoptionAlertComment GenerateAdoptionAlertComment()
	{
		return new AdoptionAlertComment()
		{
			Id = Constants.AdoptionAlertCommentData.Id,
			AdoptionAlert = Constants.AdoptionAlertCommentData.AdoptionAlert,
			AdoptionAlertId = Constants.AdoptionAlertCommentData.AdoptionAlertId,
			Content = Constants.AdoptionAlertCommentData.Content,
			User = Constants.AdoptionAlertCommentData.User,
			UserId = Constants.AdoptionAlertCommentData.UserId,
			Date = Constants.AdoptionAlertCommentData.Date
		};
	}

	public static List<AdoptionAlertComment> GenerateListOfAdoptionAlertComments()
	{
		List<AdoptionAlertComment> alertComments = new();
		for (int i = 0; i < 3; i++)
		{
			alertComments.Add(GenerateAdoptionAlertComment());
		}

		return alertComments;
	}
}