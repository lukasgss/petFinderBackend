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
			CommentOwnerId = missingAlertComment.UserId,
			Date = missingAlertComment.Date
		};
	}
}