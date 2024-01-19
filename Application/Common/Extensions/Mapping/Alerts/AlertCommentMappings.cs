using Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class AlertCommentMappings
{
	public static AlertCommentResponse ToAlertCommentResponse(this MissingAlertComment missingAlertComment)
	{
		return new AlertCommentResponse()
		{
			Id = missingAlertComment.Id,
			Content = missingAlertComment.Content,
			AlertId = missingAlertComment.MissingAlertId,
			CommentOwner = missingAlertComment.User.ToUserDataResponse(),
			Date = missingAlertComment.Date
		};
	}

	public static AlertCommentResponse ToAlertCommentResponse(this AdoptionAlertComment adoptionAlertComment)
	{
		return new AlertCommentResponse()
		{
			Id = adoptionAlertComment.Id,
			Content = adoptionAlertComment.Content,
			AlertId = adoptionAlertComment.AdoptionAlertId,
			CommentOwner = adoptionAlertComment.User.ToUserDataResponse(),
			Date = adoptionAlertComment.Date
		};
	}
}