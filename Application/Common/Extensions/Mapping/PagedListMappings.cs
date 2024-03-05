using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Application.Common.Pagination;
using Domain.Entities;
using Domain.Entities.Alerts;

namespace Application.Common.Extensions.Mapping;

public static class PagedListMappings
{
	public static PaginatedEntity<UserMessageResponse> ToUserMessageResponsePagedList(
		this PagedList<UserMessage> pagedUserMessages)
	{
		List<UserMessageResponse> userMessageResponses = pagedUserMessages
			.Select(message => message.ToUserMessageResponse())
			.ToList();

		return new PaginatedEntity<UserMessageResponse>()
		{
			Data = userMessageResponses,
			CurrentPage = pagedUserMessages.CurrentPage,
			CurrentPageCount = pagedUserMessages.CurrentPageCount,
			TotalPages = pagedUserMessages.TotalPages
		};
	}

	public static PaginatedEntity<AdoptionAlertResponse> ToAdoptionAlertResponsePagedList(
		this PagedList<AdoptionAlert> adoptionAlerts)
	{
		List<AdoptionAlertResponse> adoptionAlertResponses = adoptionAlerts
			.Select(alert => alert.ToAdoptionAlertResponse())
			.ToList();

		return new PaginatedEntity<AdoptionAlertResponse>()
		{
			Data = adoptionAlertResponses,
			CurrentPage = adoptionAlerts.CurrentPage,
			CurrentPageCount = adoptionAlerts.CurrentPageCount,
			TotalPages = adoptionAlerts.TotalPages
		};
	}

	public static PaginatedEntity<MissingAlertResponse> ToMissingAlertResponsePagedList(
		this PagedList<MissingAlert> missingAlerts)
	{
		List<MissingAlertResponse> missingAlertResponses = missingAlerts
			.Select(alert => alert.ToMissingAlertResponse())
			.ToList();

		return new PaginatedEntity<MissingAlertResponse>()
		{
			Data = missingAlertResponses,
			CurrentPage = missingAlerts.CurrentPage,
			CurrentPageCount = missingAlerts.CurrentPageCount,
			TotalPages = missingAlerts.TotalPages,
		};
	}

	public static PaginatedEntity<FoundAnimalAlertResponse> ToFoundAnimalAlertResponsePagedList(
		this PagedList<FoundAnimalAlert> foundAnimalAlerts)
	{
		List<FoundAnimalAlertResponse> missingAlertResponses = foundAnimalAlerts
			.Select(alert => alert.ToFoundAnimalAlertResponse())
			.ToList();

		return new PaginatedEntity<FoundAnimalAlertResponse>()
		{
			Data = missingAlertResponses,
			CurrentPage = foundAnimalAlerts.CurrentPage,
			CurrentPageCount = foundAnimalAlerts.CurrentPageCount,
			TotalPages = foundAnimalAlerts.TotalPages,
		};
	}

	public static PaginatedEntity<AlertCommentResponse> ToPaginatedAlertCommentResponse(
		this PagedList<MissingAlertComment> alertComments)
	{
		List<AlertCommentResponse> alertCommentResponses = alertComments
			.Select(alert => alert.ToAlertCommentResponse())
			.ToList();

		return new PaginatedEntity<AlertCommentResponse>()
		{
			Data = alertCommentResponses,
			CurrentPage = alertComments.CurrentPage,
			CurrentPageCount = alertComments.CurrentPageCount,
			TotalPages = alertComments.TotalPages
		};
	}

	public static PaginatedEntity<AlertCommentResponse> ToPaginatedAlertCommentResponse(
		this PagedList<AdoptionAlertComment> alertComments)
	{
		List<AlertCommentResponse> alertCommentResponses = alertComments
			.Select(alert => alert.ToAlertCommentResponse())
			.ToList();

		return new PaginatedEntity<AlertCommentResponse>()
		{
			Data = alertCommentResponses,
			CurrentPage = alertComments.CurrentPage,
			CurrentPageCount = alertComments.CurrentPageCount,
			TotalPages = alertComments.TotalPages
		};
	}
}