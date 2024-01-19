using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.Comments;
using Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;

namespace Application.Services.Entities.Comments;

public class AdoptionAlertCommentService : IAdoptionAlertCommentService
{
	private readonly IAdoptionAlertCommentRepository _adoptionAlertCommentRepository;
	private readonly IAdoptionAlertRepository _adoptionAlertRepository;
	private readonly IUserRepository _userRepository;
	private readonly IDateTimeProvider _dateTimeProvider;
	private readonly IGuidProvider _guidProvider;

	public AdoptionAlertCommentService(
		IAdoptionAlertCommentRepository adoptionAlertCommentRepository,
		IAdoptionAlertRepository adoptionAlertRepository,
		IUserRepository userRepository,
		IDateTimeProvider dateTimeProvider,
		IGuidProvider guidProvider)
	{
		_adoptionAlertCommentRepository = adoptionAlertCommentRepository ??
		                                  throw new ArgumentNullException(nameof(adoptionAlertCommentRepository));
		_adoptionAlertRepository = adoptionAlertRepository
		                           ?? throw new ArgumentNullException(nameof(adoptionAlertRepository));
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
		_guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
	}

	public async Task<AlertCommentResponse> GetCommentByIdAsync(Guid commentId)
	{
		AdoptionAlertComment? alertComment = await _adoptionAlertCommentRepository.GetByIdAsync(commentId);
		if (alertComment is null)
		{
			throw new NotFoundException("Comentário com o id especificado não existe.");
		}

		return alertComment.ToAlertCommentResponse();
	}

	public async Task<PaginatedEntity<AlertCommentResponse>> ListCommentsAsync(
		Guid alertId, int pageNumber, int pageSize)
	{
		var alertComments =
			await _adoptionAlertCommentRepository.GetCommentsByAlertIdAsync(alertId, pageNumber, pageSize);

		return alertComments.ToPaginatedAlertCommentResponse();
	}

	public async Task<AlertCommentResponse> PostCommentAsync(
		CreateAlertCommentRequest alertRequest, Guid userId, Guid routeId)
	{
		if (alertRequest.AlertId != routeId)
		{
			throw new BadRequestException("Id da rota não coincide com o id especificado.");
		}

		AdoptionAlert? commentedAlert = await _adoptionAlertRepository.GetByIdAsync(alertRequest.AlertId);
		if (commentedAlert is null)
		{
			throw new NotFoundException("Alerta com o id especificado não existe.");
		}

		User? commentOwner = await _userRepository.GetUserByIdAsync(userId);

		AdoptionAlertComment alertComment = new()
		{
			Id = _guidProvider.NewGuid(),
			Content = alertRequest.Content,
			User = commentOwner!,
			UserId = commentOwner!.Id,
			AdoptionAlert = commentedAlert,
			AdoptionAlertId = commentedAlert.Id,
			Date = _dateTimeProvider.UtcNow()
		};

		_adoptionAlertCommentRepository.Add(alertComment);
		await _adoptionAlertCommentRepository.CommitAsync();

		return alertComment.ToAlertCommentResponse();
	}

	public async Task DeleteCommentAsync(Guid commentId, Guid userId)
	{
		AdoptionAlertComment? commentToDelete = await _adoptionAlertCommentRepository.GetByIdAsync(commentId);
		if (commentToDelete is null)
		{
			throw new NotFoundException("Comentário com o id especificado não existe.");
		}

		if (commentToDelete.UserId != userId)
		{
			throw new UnauthorizedException("Não é possível excluir comentários de outros usuários.");
		}

		_adoptionAlertCommentRepository.Delete(commentToDelete);
		await _adoptionAlertCommentRepository.CommitAsync();
	}
}