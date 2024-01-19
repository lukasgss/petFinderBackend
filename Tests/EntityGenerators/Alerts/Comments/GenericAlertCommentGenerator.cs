using System.Collections.Generic;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators.Alerts.Comments;

public static class GenericAlertCommentGenerator
{
	public static AlertCommentResponse GenerateMissingAlertCommentResponse()
	{
		return new AlertCommentResponse()
		{
			Id = Constants.MissingAlertCommentData.Id,
			Content = Constants.MissingAlertCommentData.Content,
			Date = Constants.MissingAlertCommentData.Date,
			AlertId = Constants.MissingAlertCommentData.MissingAlertId,
			CommentOwner = UserGenerator.GenerateUser().ToUserDataResponse()
		};
	}

	public static AlertCommentResponse GenerateAdoptionAlertCommentResponse()
	{
		return new AlertCommentResponse()
		{
			Id = Constants.AdoptionAlertCommentData.Id,
			Content = Constants.AdoptionAlertCommentData.Content,
			Date = Constants.AdoptionAlertCommentData.Date,
			AlertId = Constants.AdoptionAlertCommentData.AdoptionAlertId,
			CommentOwner = UserGenerator.GenerateUser().ToUserDataResponse()
		};
	}

	public static CreateAlertCommentRequest GenerateCreateAlertCommentRequest()
	{
		return new CreateAlertCommentRequest()
		{
			Content = Constants.MissingAlertCommentData.Content,
			AlertId = Constants.MissingAlertCommentData.MissingAlertId
		};
	}

	public static List<AlertCommentResponse> GenerateListOfMissingAlertComment()
	{
		List<AlertCommentResponse> alertComments = new();
		for (int i = 0; i < 3; i++)
		{
			alertComments.Add(GenerateMissingAlertCommentResponse());
		}

		return alertComments;
	}

	public static List<AlertCommentResponse> GenerateListOfAdoptionAlertComment()
	{
		List<AlertCommentResponse> alertComments = new();
		for (int i = 0; i < 3; i++)
		{
			alertComments.Add(GenerateAdoptionAlertCommentResponse());
		}

		return alertComments;
	}
}