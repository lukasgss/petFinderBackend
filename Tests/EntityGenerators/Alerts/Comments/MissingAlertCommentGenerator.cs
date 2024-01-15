using System.Collections.Generic;
using Domain.Entities;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators.Alerts.Comments;

public static class MissingAlertCommentGenerator
{
	public static MissingAlertComment GenerateMissingAlertComment()
	{
		return new MissingAlertComment()
		{
			Id = Constants.MissingAlertCommentData.Id,
			Content = Constants.MissingAlertCommentData.Content,
			Date = Constants.MissingAlertCommentData.Date,
			User = Constants.MissingAlertCommentData.User,
			UserId = Constants.MissingAlertCommentData.UserId,
			MissingAlert = Constants.MissingAlertCommentData.MissingAlert,
			MissingAlertId = Constants.MissingAlertCommentData.MissingAlertId
		};
	}

	public static List<MissingAlertComment> GenerateListOfMissingAlertComment()
	{
		List<MissingAlertComment> alertComments = new();
		for (int i = 0; i < 3; i++)
		{
			alertComments.Add(GenerateMissingAlertComment());
		}

		return alertComments;
	}
}