using Application.ApplicationConstants;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Alerts.Comments;
using Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Validations.Alerts.AlertComments;
using Application.Common.Validations.Errors;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Comments;

[ApiController]
[Route("/api/comments/missing-alert")]
public class MissingAlertCommentController : ControllerBase
{
	private readonly IMissingAlertCommentService _missingAlertCommentService;
	private readonly IUserAuthorizationService _userAuthorizationService;

	public MissingAlertCommentController(IMissingAlertCommentService missingAlertCommentService,
		IUserAuthorizationService userAuthorizationService)
	{
		_missingAlertCommentService = missingAlertCommentService ??
		                              throw new ArgumentNullException(nameof(missingAlertCommentService));
		_userAuthorizationService = userAuthorizationService ??
		                            throw new ArgumentNullException(nameof(userAuthorizationService));
	}

	[Authorize]
	[HttpGet("{alertId:guid}", Name = "GetCommentById")]
	public async Task<ActionResult<AlertCommentResponse>> GetCommentById(Guid alertId)
	{
		return await _missingAlertCommentService.GetAlertCommentByIdAsync(alertId);
	}

	[HttpGet("list/{alertId:guid}")]
	public async Task<ActionResult<PaginatedEntity<AlertCommentResponse>>> GetCommentsFromAlert(
		Guid alertId, int pageNumber = 1, int pageSize = Constants.DefaultPageSize)
	{
		var commentsInAlert =
			await _missingAlertCommentService.ListMissingAlertCommentsAsync(alertId, pageNumber, pageSize);
		return Ok(commentsInAlert);
	}

	[Authorize]
	[HttpPost("{alertId:guid}")]
	public async Task<ActionResult<AlertCommentResponse>> Comment(Guid alertId, CreateAlertCommentRequest alert)
	{
		CreateAlertCommentValidator requestValidator = new();
		ValidationResult validationResult = requestValidator.Validate(alert);
		if (!validationResult.IsValid)
		{
			var errors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
			return BadRequest(errors);
		}

		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		AlertCommentResponse createdComment =
			await _missingAlertCommentService.PostCommentAsync(alert, userId, alertId);

		return new CreatedAtRouteResult(
			nameof(GetCommentById), new { alertId = createdComment.AlertId }, createdComment);
	}

	[Authorize]
	[HttpDelete("{commentId:guid}")]
	public async Task<ActionResult> DeleteComment(Guid commentId)
	{
		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);
		await _missingAlertCommentService.DeleteCommentAsync(commentId, userId);

		return NoContent();
	}
}