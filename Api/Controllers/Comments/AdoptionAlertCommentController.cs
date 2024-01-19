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
[Route("/api/comments/adoption-alert")]
public class AdoptionAlertCommentController : ControllerBase
{
	private readonly IAdoptionAlertCommentService _adoptionAlertCommentService;
	private readonly IUserAuthorizationService _userAuthorizationService;

	public AdoptionAlertCommentController(
		IUserAuthorizationService userAuthorizationService,
		IAdoptionAlertCommentService adoptionAlertCommentService)
	{
		_userAuthorizationService = userAuthorizationService ??
		                            throw new ArgumentNullException(nameof(userAuthorizationService));
		_adoptionAlertCommentService = adoptionAlertCommentService;
	}

	[HttpGet("{alertId:guid}", Name = "GetAlertCommentById")]
	public async Task<ActionResult<AlertCommentResponse>> GetAlertCommentById(Guid alertId)
	{
		return await _adoptionAlertCommentService.GetCommentByIdAsync(alertId);
	}

	[HttpGet("list/{alertId:guid}")]
	public async Task<ActionResult<PaginatedEntity<AlertCommentResponse>>> GetCommentsFromAlert(
		Guid alertId, int pageNumber = 1, int pageSize = Constants.DefaultPageSize)
	{
		var commentsInAlert = await _adoptionAlertCommentService.ListCommentsAsync(alertId, pageNumber, pageSize);
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
			await _adoptionAlertCommentService.PostCommentAsync(alert, userId, alertId);

		return new CreatedAtRouteResult(
			nameof(GetAlertCommentById), new { alertId = createdComment.Id }, createdComment);
	}

	[Authorize]
	[HttpDelete("{commentId:guid}")]
	public async Task<ActionResult> DeleteComment(Guid commentId)
	{
		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);
		await _adoptionAlertCommentService.DeleteCommentAsync(commentId, userId);

		return NoContent();
	}
}