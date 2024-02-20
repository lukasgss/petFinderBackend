using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.Comments;
using Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;

namespace Application.Services.Entities.Comments;

public class MissingAlertCommentService : IMissingAlertCommentService
{
	private readonly IMissingAlertCommentRepository _missingAlertCommentRepository;
	private readonly IMissingAlertRepository _missingAlertRepository;
	private readonly IUserRepository _userRepository;
	private readonly IValueProvider _valueProvider;

	public MissingAlertCommentService(
		IMissingAlertCommentRepository missingAlertCommentRepository,
		IMissingAlertRepository missingAlertRepository,
		IUserRepository userRepository,
		IValueProvider valueProvider)
	{
		_missingAlertCommentRepository =
			missingAlertCommentRepository ?? throw new ArgumentNullException(nameof(missingAlertCommentRepository));
		_missingAlertRepository =
			missingAlertRepository ?? throw new ArgumentNullException(nameof(missingAlertRepository));
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
	}

	public async Task<AlertCommentResponse> GetCommentByIdAsync(Guid commentId)
	{
		MissingAlertComment? alertComment = await _missingAlertCommentRepository.GetByIdAsync(commentId);
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
			await _missingAlertCommentRepository.GetCommentsByAlertIdAsync(alertId, pageNumber, pageSize);

		return alertComments.ToPaginatedAlertCommentResponse();
	}

	public async Task<AlertCommentResponse> PostCommentAsync(
		CreateAlertCommentRequest alertRequest, Guid userId, Guid routeId)
	{
		if (alertRequest.AlertId != routeId)
		{
			throw new BadRequestException("Id da rota não coincide com o id especificado.");
		}

		MissingAlert? commentedAlert = await _missingAlertRepository.GetByIdAsync(alertRequest.AlertId);
		if (commentedAlert is null)
		{
			throw new NotFoundException("Alerta com o id especificado não existe.");
		}

		User? commentOwner = await _userRepository.GetUserByIdAsync(userId);

		MissingAlertComment alertComment = new()
		{
			Id = _valueProvider.NewGuid(),
			Content = alertRequest.Content,
			User = commentOwner!,
			UserId = commentOwner!.Id,
			MissingAlert = commentedAlert,
			MissingAlertId = commentedAlert.Id,
			Date = _valueProvider.UtcNow()
		};

		_missingAlertCommentRepository.Add(alertComment);
		await _missingAlertRepository.CommitAsync();

		return alertComment.ToAlertCommentResponse();
	}

	public async Task DeleteCommentAsync(Guid commentId, Guid userId)
	{
		MissingAlertComment? commentToDelete = await _missingAlertCommentRepository.GetByIdAsync(commentId);
		if (commentToDelete is null)
		{
			throw new NotFoundException("Comentário com o id especificado não existe.");
		}

		if (commentToDelete.UserId != userId)
		{
			throw new UnauthorizedException("Não é possível excluir comentários de outros usuários.");
		}

		_missingAlertCommentRepository.Delete(commentToDelete);
		await _missingAlertCommentRepository.CommitAsync();
	}
}